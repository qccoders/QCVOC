/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React, { Component } from 'react';
import PropTypes from 'prop-types';
import moment from 'moment';

import { withStyles } from '@material-ui/core/styles';
import { 
    Card, 
    CardContent, 
    Typography, 
    CircularProgress, 
    Button,
    Fab,
} from '@material-ui/core';
import { SpeakerPhone, Today, Shop } from '@material-ui/icons';

import { CHECKIN_SERVICE_ID } from '../constants';
import { isMobileAttached, initiateMobileScan } from '../mobile';
import { withContext } from '../shared/ContextProvider';
import { getScanResult } from './scannerUtil';
import { userCanView } from '../util';
import ServiceList from '../services/ServiceList';
import ScannerMenu from './ScannerMenu';
import ScanDisplay from './ScanDisplay';
import EventList from '../events/EventList';
import ContentWrapper from '../shared/ContentWrapper';
import ScannerHistoryDialog from './ScannerHistoryDialog';
import ManualScanDialog from './ManualScanDialog';
import PlusOneDialog from './PlusOneDialog';

const historyLimit = 5;

const styles = {
    fab: {
        margin: 0,
        top: 'auto',
        right: 20,
        bottom: 20,
        left: 'auto',
        position: 'fixed',
        zIndex: 1000,
    },
    card: {
        height: 'calc(100vh - 115px)',
        maxWidth: 800,
        margin: 'auto',
    },
    refreshSpinner: {
        position: 'fixed',
        left: 0,
        right: 0,
        marginLeft: 'auto',
        marginRight: 'auto',
        marginTop: 27,
    },
    displayBox: {
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        textAlign: 'center',
        height: 'calc(100vh - 188px)',
        width: '100%',
    },
    title: {
        display: 'inline',
    },
};

const initialState = {
    loadApi: {
        isExecuting: false,
        isErrored: false,
    },
    refreshApi: {
        isExecuting: false,
        isErrored: false,
    },
    scanApi: {
        isExecuting: false,
        isErrored: false,
    },
    scanner: {
        event: undefined,
        service: undefined,
        history: [],
    },
    scan: {
        cardNumber: undefined,
        status: undefined,
        response: undefined,
        plusOne: undefined,
    },
    events: [],
    services: [],
    plusOne: undefined,
    historyDialog: {
        open: false,
    },
    scanDialog: {
        open: false,
    },
    plusOneDialog: {
        open: false,
        intent: undefined,
    },
};

class Scanner extends Component {
    state = initialState;

    componentDidMount = () => {
        window.inputBarcodeScanner = this.handleBarcodeScanned;

        let scanner = undefined;

        try {
            scanner = JSON.parse(sessionStorage.getItem('scanner'));

            if (scanner === undefined || scanner.event === undefined || scanner.service === undefined || scanner.history === undefined) {
                throw scanner;
            }
        } catch {
            sessionStorage.removeItem('scanner');
        }

        if (scanner && scanner.event && scanner.service) {
            this.setState({ scanner: scanner });
        }
        else {
            this.fetchEvents('refreshApi');
        }
    }

    handleBarcodeScanned = (barcode) => {
        if (barcode === undefined) return;

        let { event, service } = this.state.scanner;
        let scan = { eventId: event && event.id, serviceId: service && service.id, cardNumber: barcode, plusOne: this.state.plusOne === undefined ? false : this.state.plusOne };

        if (scan.serviceId !== CHECKIN_SERVICE_ID) {
            this.setState({
                scan: scan,
                scanApi: { ...this.state.scanApi, isExecuting: true },
            }, () => {
                this.props.context.api.get('/v1/scans/' + scan.eventId + '/' + scan.cardNumber + '/checkin')
                .then(response => {
                    if (response.data.plusOne) {
                        this.setState({ 
                            scanApi: { isExecuting: false, isErrored: false },
                            plusOneDialog: { open: true, intent: 'service' },
                        });
                    }
                    else {
                        this.sendScan(scan, barcode);
                    }
                }, error => {
                    this.setState({ scanApi: { isExecuting: false, isErrored: true }}, () => {
                        this.handleScanResponse(barcode, error.response);
                    });
                });
            });
        }
        else {
            this.sendScan(scan, barcode);
        }
    }

    sendScan = (scan, barcode) => {
        this.setState({ 
            scan: initialState.scan,
            scanDialog: { open: false },
            scanApi: { ...this.state.scanApi, isExecuting: true },
        }, () => {
            this.props.context.api.put('/v1/scans', scan)
            .then(response => {
                this.setState({ scanApi: { isExecuting: false, isErrored: false }}, () => {
                    this.handleScanResponse(barcode, response);
                });
            }, error => {
                this.setState({ scanApi: { isExecuting: false, isErrored: true }}, () => {
                    this.handleScanResponse(barcode, error.response);
                });
            });
        });
    }

    handleScanClick = () => {
        if (this.state.scanner.service && this.state.scanner.service.id === CHECKIN_SERVICE_ID) { 
            this.setState({ plusOneDialog: { open: true, intent: 'checkin' }});
        }
        else {
            this.scan();
        }
    }

    scan = () => {
        if (isMobileAttached()) {
            initiateMobileScan("window.inputBarcodeScanner");
        }
        else {
            this.setState({ scanDialog: { open: true }});
        }
    }

    handleScanDialogClose = (result) => {
        this.setState({ scanDialog: { open: false }}, () => {
            if (result !== undefined) {
                this.handleBarcodeScanned(result);
            }
        });
    }

    handlePlusOneDialogClose = (result) => {
        let intent = this.state.plusOneDialog.intent;

        this.setState({ 
            plusOneDialog: { open: false, intent: undefined }, 
            plusOne: result === undefined ? false : result, 
        }, () => {
            if (result !== undefined) {
                if (intent === 'checkin') {
                    // for checkin scans, +1 selection comes before card entry
                    this.scan();
                }
                else if (intent === 'service') {
                    // for service scans, +1 selection comes after card entry, and only if the veteran
                    // checked in with a +1.
                    this.sendScan({ ...this.state.scan, plusOne: result });
                }
            }
        });
    }

    handleScanResponse = (cardNumber, response) => {
        let scan = { cardNumber: cardNumber, status: response.status, response: response.data };

        let historyScan = JSON.parse(JSON.stringify(scan));
        if (historyScan && historyScan.response && historyScan.response.veteran && historyScan.response.veteran.photoBase64) {
            delete historyScan.response.veteran.photoBase64;
        }

        let history = this.state.scanner.history.slice(0);
        history.unshift(historyScan);
        history = history.slice(0, historyLimit);

        this.setState({ 
            scan: scan,
            scanner: { ...this.state.scanner, history: history },
        }, () => {
            sessionStorage.setItem('scanner', JSON.stringify(this.state.scanner));
        });
    }

    resetScanner = (resolve) => { 
        this.setState({ ...initialState }, () => {
            sessionStorage.removeItem('scanner');

            this.fetchEvents('refreshApi')
            .then(() => resolve());
        });
    }

    clearLastScan = () => {
        this.setState({ scan: initialState.scan });
    }

    deleteScan = (scan) => {
        this.setState({ 
            scanner: { ...this.state.scanner, history: this.state.scanner.history.filter(oldScan => oldScan.cardNumber !== scan.cardNumber) },
        }, () => {
            sessionStorage.setItem('scanner', JSON.stringify(this.state.scanner));

            if (this.state.scan.cardNumber === scan.cardNumber) {
                this.clearLastScan();
            }
        });
    }

    fetchEvents = (apiType) => {
        let start = moment().startOf('day').format();
        let end = moment().endOf('day').format();

        return new Promise((resolve, reject) => { 
            this.setState({ [apiType]: { ...this.state[apiType], isExecuting: true }}, () => {
                this.props.context.api.get('/v1/events?dateStart=' + start + '&dateEnd=' + end)
                .then(response => {
                    this.setState({ 
                        events: response.data,
                        [apiType]: { isExecuting: false, isErrored: false },
                    }, () => resolve());
                })
                .catch(error => {
                    this.setState({ [apiType]: { isExecuting: false, isErrored: true }}, () => reject());
                });
            });
        });
    }

    fetchServices = (apiType) => {
        this.setState({ [apiType]: { ...this.state[apiType], isExecuting: true }}, () => {
            this.props.context.api.get('/v1/services')
            .then(response => {
                this.setState({ 
                    services: response.data,
                    [apiType]: { isExecuting: false, isErrored: false },
                });
            })
            .catch(error => {
                this.setState({ [apiType]: { isExecuting: false, isErrored: true }});
            });
        });
    }

    getTitle = (scanner) => {
        let { event, service } = scanner;

        if (event === undefined) return 'Select Event';
        if (service === undefined) return 'Select Service';
        return (scanner.service ? scanner.service.name + ' ' : '') + 'Scanner';
    }

    getDailyEvent = () => {
        let start = moment().startOf('day').add(8, 'hours');
        let end = moment().startOf('day').add(8, 'hours').add(7, 'hours');

        return {
            id: undefined,
            name: 'Daily Event for ' + start.format('M/DD/YY'),
            startDate: start.format(),
            endDate: end.format(),
        };
    }

    handleEventItemClick = (event) => {
        new Promise((resolve, reject) => {
            if (event.id === undefined) {
                this.setState({ 
                    refreshApi: { ...this.state.refreshApi, isExecuting: true },                   
                }, () => {
                    this.props.context.api.post('/v1/events', event)
                    .then(response => {
                        resolve(response.data);
                    })
                    .catch(error => {
                        reject(error.response);
                    });
                });
            }
            else {
                resolve(event);
            }
        }).then(event => {
            this.setState({ scanner: { ...this.state.scanner, event: event }}, () => {
                this.fetchServices('refreshApi');
            });
        });
    }

    handleServiceItemClick = (service) => {
        let scanner = { ...this.state.scanner, service: service };

        this.setState({ scanner: scanner }, () => {
            sessionStorage.setItem('scanner', JSON.stringify(scanner));
        });
    }

    render() {
        let classes = this.props.classes;
        let { loadApi, refreshApi, scanApi, scanner, scan, events, services, historyDialog, scanDialog, plusOneDialog } = this.state;

        let title = this.getTitle(scanner);

        let eventSelected = scanner.event !== undefined;
        let serviceSelected = scanner.service !== undefined;

        let dailyEvent = this.getDailyEvent();
        let dailyEventExists = events.find(e => e.name === dailyEvent.name);

        if (dailyEventExists === undefined && userCanView()) {
            events = events.concat(dailyEvent);
        }

        let scanResult = getScanResult(scan);

        return (
            <div className={classes.root}>
                <ContentWrapper api={loadApi}>
                    <Card className={classes.card} style={{ backgroundColor: (scanResult && scanResult.color) || undefined }}>
                        <CardContent>
                            <div>
                                {/* todo: move this to a component */}
                                <Typography gutterBottom variant="h5" className={classes.title}>
                                    {title}
                                </Typography>
                                <ScannerMenu 
                                    visible={scanner.event !== undefined}
                                    configured={scanner.event !== undefined && scanner.service !== undefined}
                                    lastScan={scan}
                                    clearLastScan={this.clearLastScan}
                                    resetScanner={this.resetScanner}
                                    viewHistory={() => this.setState({ historyDialog: { open: true }})}
                                />
                            </div>
                            {refreshApi.isExecuting ?
                                <CircularProgress size={30} color={'secondary'} className={classes.refreshSpinner}/> :
                                <div>
                                    {!eventSelected && 
                                        <EventList
                                            events={events}
                                            icon={<Today/>}
                                            onItemClick={this.handleEventItemClick}
                                        />
                                    }
                                    {!serviceSelected && eventSelected && 
                                        <ServiceList
                                            services={services}
                                            icon={<Shop/>}
                                            onItemClick={this.handleServiceItemClick}
                                        />
                                    }
                                    {serviceSelected && eventSelected &&
                                        <div className={classes.displayBox}>
                                            {scanApi.isExecuting ? <CircularProgress thickness={5} size={60} color={'secondary'}/> :
                                                !scan.status ? <Button disabled>Ready to Scan</Button> :
                                                    <ScanDisplay scan={scan}/>
                                            }
                                        </div>
                                    }
                                </div>
                            }
                        </CardContent>
                    </Card>
                    {serviceSelected && <Fab
                        color="secondary" 
                        className={classes.fab}
                        onClick={this.handleScanClick}
                    >
                        <SpeakerPhone/>
                    </Fab>}
                    <ManualScanDialog
                        open={scanDialog.open}
                        onClose={this.handleScanDialogClose}
                    />
                    <PlusOneDialog
                        open={plusOneDialog.open}
                        onClose={this.handlePlusOneDialogClose}
                    />
                    <ScannerHistoryDialog
                        open={historyDialog.open}
                        service={scanner && scanner.service ? scanner.service.name : ''}
                        history={scanner.history}
                        onDelete={this.deleteScan}
                        onClose={() => this.setState({ historyDialog: { open: false }})}
                    />
                </ContentWrapper>
            </div>
        );
    }
}

Scanner.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(withContext(Scanner)); 