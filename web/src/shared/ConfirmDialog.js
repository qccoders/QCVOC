/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/
import React, { Component } from 'react';
import PropTypes from 'prop-types';

import Button from '@material-ui/core/Button';
import DialogTitle from '@material-ui/core/DialogTitle';
import DialogContent from '@material-ui/core/DialogContent';
import DialogActions from '@material-ui/core/DialogActions';
import Dialog from '@material-ui/core/Dialog';
import CircularProgress from '@material-ui/core/CircularProgress';

const initialState = {
    api: {
        isExecuting: false,
        isErrored: false,
    }
}

const styles = {
    spinner: {
        position: 'fixed',
    }
}

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
                if (!this.props.suppressCloseOnConfirm) {
                    this.setState({ api: { isExecuting: false, isErrored: false }});
                    this.props.onClose({ cancelled: false }) 
                }
            }, error => {
                this.setState({ api: { isExecuting: false, isErrored: true }});
                this.props.onClose({ cancelled: false }) 
            })
        })
    }

    handleCancelClick = () => {
        this.setState(initialState, () => this.props.onClose({ cancelled: true }))
    }

    render() {
        let additionalProps = { ...this.props };
        delete additionalProps.onConfirm;
        delete additionalProps.suppressCloseOnConfirm;

        return (
            <Dialog
                {...additionalProps}
            >
                <DialogTitle>{this.props.title}</DialogTitle>
                <DialogContent>
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

export default ConfirmDialog;