/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React, { Component } from 'react';
import PropTypes from 'prop-types';
import api from '../api';

import { withStyles } from '@material-ui/core/styles';
import ContentWrapper from '../shared/ContentWrapper';
import { Card, CardContent, Typography, CircularProgress } from '@material-ui/core';
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

class Scanner extends Component {
    state = {
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
                                    Scan stuff here
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