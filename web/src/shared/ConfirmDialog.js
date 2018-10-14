/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/
import React, { Component } from 'react';
import PropTypes from 'prop-types';

import { withStyles } from '@material-ui/core/styles';
import {
    Button,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    CircularProgress,
} from '@material-ui/core';

const initialState = {
    api: {
        isExecuting: false,
        isErrored: false,
    },
};

const styles = {
    spinner: {
        position: 'fixed',
    },
    dialog: {
        width: 320,
        marginLeft: 'auto',
        marginRight: 'auto',
        marginTop: 50,
        height: 'fit-content',
    },
};

class ConfirmDialog extends Component {
    state = initialState;

    componentWillReceiveProps = (nextProps) => {
        if (!this.props.open && nextProps.open) {
            this.setState(initialState);
        }
    }

    handleConfirmClick = () => {
        this.setState({ api: { ...this.state.api, isExecuting: true }}, () => {
            this.props.onConfirm()
            .then(response => {
                this.setState({ api: { isExecuting: false, isErrored: false }}, () => {
                    if (!this.props.suppressCloseOnConfirm) {
                        this.props.onClose({ cancelled: false }); 
                    }
                });
            }, error => {
                this.setState({ api: { isExecuting: false, isErrored: true }});
                this.props.onClose({ cancelled: false }); 
            });
        });
    }

    handleCancelClick = () => {
        this.setState(initialState, () => this.props.onClose({ cancelled: true }));
    }

    render() {
        let additionalProps = { ...this.props };
        let { api } = this.state;
        
        delete additionalProps.classes;
        delete additionalProps.onConfirm;
        delete additionalProps.suppressCloseOnConfirm;

        let dim = api.isExecuting ? { opacity: 0.5 } : undefined;

        return (
            <Dialog
                PaperProps={{ className: this.props.classes.dialog }}
                {...additionalProps}
                scroll={'body'}
            >
                <DialogTitle style={dim}>{this.props.title}</DialogTitle>
                <DialogContent style={dim}>
                    {this.props.children}
                </DialogContent>
                <DialogActions>
                    <Button 
                        disabled={this.state.api.isExecuting} 
                        onClick={this.handleCancelClick} color="primary"
                    >
                        Cancel
                    </Button>
                    <Button 
                        disabled={this.state.api.isExecuting}
                        onClick={this.handleConfirmClick} color="primary"
                    >
                        {this.state.api.isExecuting && <CircularProgress size={20} style={styles.spinner}/>}
                        {this.props.prompt}
                    </Button>
                </DialogActions>
            </Dialog>
        );
    }
}

ConfirmDialog.propTypes = {
    title: PropTypes.string.isRequired,
    onClose: PropTypes.func.isRequired,
    onConfirm: PropTypes.func.isRequired,
    children: PropTypes.object.isRequired,
    prompt: PropTypes.string.isRequired,
    suppressCloseOnConfirm: PropTypes.bool,
};

export default withStyles(styles)(ConfirmDialog);