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

const styles = {
    card: {
        minHeight: 272,
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
    }

    render() {
        let classes = this.props.classes;

        return (
            <div className={classes.root}>
                <ContentWrapper api={loadApi}>
                    <Card className={classes.card}>
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