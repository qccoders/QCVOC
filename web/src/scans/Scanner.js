/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React, { Component } from 'react';
import PropTypes from 'prop-types';
import api from '../api';

import { withStyles } from '@material-ui/core/styles';
import ContentWrapper from '../shared/ContentWrapper';
import { Card, CardContent, Typography, CircularProgress, Button } from '@material-ui/core';
import { red, green, orange } from '@material-ui/core/colors';

const styles = {
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
    scan: {
        result: undefined,
        message: undefined,
    },
}

class Scanner extends Component {
    state = initialState;

    getScanColor = (result) => {
        switch(result) {
            case undefined:
                return undefined;
            case 201:
                return green['A700'];
            case 200:
                return orange['A700'];
            default:
                return red['A700'];
        }
    }

    handleScan = (result, message) => {
        this.setState({ scan: { result: result, message: message }}, () => {
            setTimeout(() => {
                this.setState({ scan: initialState.scan });
            }, 2500);
        });
    }

    render() {
        let classes = this.props.classes;
        let { loadApi, refreshApi, scan } = this.state;

        let color = this.getScanColor(scan.result);

        return (
            <div className={classes.root}>
                <ContentWrapper api={loadApi}>
                    <Card className={classes.card} style={{backgroundColor: color}}>
                        <CardContent>
                            <Typography gutterBottom variant="headline" component="h2">
                                Scanner
                            </Typography>
                            {refreshApi.isExecuting ?
                                <CircularProgress size={30} color={'secondary'} className={classes.refreshSpinner}/> :
                                <div>
                                    <Button onClick={() => this.handleScan(200, 'Already scanned')}>200</Button>
                                    <Button onClick={() => this.handleScan(201, 'Scan created')}>201</Button>
                                    <Button onClick={() => this.handleScan(403, 'Bad scan')}>403</Button>
                                </div>
                            }
                        </CardContent>
                    </Card>
                </ContentWrapper>
            </div>
        );
    }
}

Scanner.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Scanner); 