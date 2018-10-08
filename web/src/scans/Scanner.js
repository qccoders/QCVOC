/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React, { Component } from 'react';
import PropTypes from 'prop-types';
import api from '../api';
import moment from 'moment';

import { withStyles } from '@material-ui/core/styles';
import ContentWrapper from '../shared/ContentWrapper';
import { Card, CardContent, Typography, CircularProgress, Button } from '@material-ui/core';
import { SpeakerPhone, Today, Shop } from '@material-ui/icons';
import { isMobileAttached, initiateMobileScan } from '../mobile';
import EventList from '../events/EventList';
import ServiceList from '../services/ServiceList';
import ScannerMenu from './ScannerMenu';

import ScannerHistoryDialog from './ScannerHistoryDialog';
import { getScanResult } from './scannerUtil';
import ManualScanDialog from './ManualScanDialog';

const historyLimit = 5;

const styles = {
    fab: {
        margin: 0,
        top: 'auto',
        right: 20,
        bottom: 20,
        left: 'auto',
        position: 'fixed',
        zIndex: 1000
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
        marginTop: 68,
    },
    displayBox: {
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        height: 'calc(100vh - 188px)',
    },
    title: {
        display: 'inline',
    }
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
    scanner: {
        event: undefined,
        service: undefined,
    },
    scan: {
        status: undefined,
        data: undefined,
    },
    events: [],
    services: [],
    history: [],
    historyDialog: {
        open: false,
    },
    scanDialog: {
        open: false,
    },
}

class Scanner extends Component {
    state = initialState;

    componentDidMount = () => {
        window.barcodeScanned = this.handleBarcodeScanned;

        this.fetchEvents('refreshApi');
    }

    handleBarcodeScanned = (barcode) => {
        let { event, service } = this.state.scanner;
        let scan = { eventId: event && event.id, serviceId: service && service.id, cardNumber: barcode };

        api.put('/v1/scans', scan)
        .then(response => {
            this.handleScanResponse(barcode, response);
        }, error => {
            this.handleScanResponse(barcode, error.response);
        });
    }

    handleScanClick = () => {
        if (isMobileAttached()) {
            initiateMobileScan();
        }
        else {
            this.setState({ scanDialog: { open: true }});
        }
    }

    handleScanResponse = (cardNumber, response) => {
        let scan = { cardNumber: cardNumber, status: response.status, response: response.data };

        let history = this.state.history.slice(0);
        history.unshift(scan);
        history = history.slice(0, historyLimit);

        this.setState({ 
            scan: scan,
            history: history,
        }, () => {
            setTimeout(() => {
                this.setState({ scan: initialState.scan });
            }, 2500);
        });
    }

    resetScanner = (resolve) => { 
        this.fetchEvents('refreshApi')
        .then(() => {
            this.setState({ scanner: initialState.scanner }, () => resolve());
        });
    }

    fetchEvents = (apiType) => {
        let start = moment().startOf('day').format();
        let end = moment().endOf('day').format();

        return new Promise((resolve, reject) => { 
            this.setState({ [apiType]: { ...this.state[apiType], isExecuting: true }}, () => {
                api.get('/v1/events?dateStart=' + start + '&dateEnd=' + end)
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
            api.get('/v1/services')
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

    getScanDisplay = (scan) => {
        return <pre>{JSON.stringify(scan, null, 2)}</pre>;
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
                    refreshApi: { ...this.state.refreshApi, isExecuting: true }                   
                }, () => {
                    api.post('/v1/events', event)
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
        this.setState({ scanner: { ...this.state.scanner, service: service }});
    }

    render() {
        let classes = this.props.classes;
        let { loadApi, refreshApi, scanner, scan, events, services, history, historyDialog, scanDialog } = this.state;

        let title = this.getTitle(scanner);
        let display = this.getScanDisplay(scan);

        let eventSelected = scanner.event !== undefined;
        let serviceSelected = scanner.service !== undefined;

        let dailyEvent = this.getDailyEvent();
        let dailyEventExists = events.find(e => e.name === dailyEvent.name);

        if (dailyEventExists === undefined) {
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
                                <Typography gutterBottom variant="headline" component="h2" className={classes.title}>
                                    {title}
                                </Typography>
                                <ScannerMenu 
                                    visible={scanner.event !== undefined}
                                    configured={scanner.event !== undefined && scanner.service !== undefined}
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
                                            {!scan.status ? <Button disabled>Ready to Scan</Button> :
                                                display
                                            }
                                        </div>
                                    }
                                </div>
                            }
                        </CardContent>
                    </Card>
                    {serviceSelected && <Button 
                        variant="fab" 
                        color="secondary" 
                        className={classes.fab}
                        onClick={this.handleScanClick}
                    >
                        <SpeakerPhone/>
                    </Button>}
                    <ManualScanDialog
                        open={scanDialog.open}
                        onClose={() => this.setState({ scanDialog: { open: false }})}
                    />
                    <ScannerHistoryDialog
                        open={historyDialog.open}
                        history={history}
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

export default withStyles(styles)(Scanner); 