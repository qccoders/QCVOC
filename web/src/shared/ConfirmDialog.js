import React, { Component } from 'react';
import PropTypes from 'prop-types';

import Button from '@material-ui/core/Button';
import DialogTitle from '@material-ui/core/DialogTitle';
import DialogContent from '@material-ui/core/DialogContent';
import DialogActions from '@material-ui/core/DialogActions';
import Dialog from '@material-ui/core/Dialog';
import { CircularProgress } from '../../node_modules/@material-ui/core';
import grey from '@material-ui/core/colors/grey';

const initialState = {
    api: {
        isExecuting: false,
        isErrored: false,
    }
}

const styles = {
    spinner: {
        position: 'fixed',
        left: 0,
        right: 0,
        top: 0,
        bottom: 0,
        marginLeft: 'auto',
        marginRight: 'auto',
        marginTop: 'auto',
        marginBottom: 'auto',
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
                    this.setState({ api: { isExecuting: false, isErrored: true }});
                    this.props.onClose({ cancelled: false }) 
                }
            }, error => { });
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
                PaperProps={{style: this.state.api.isExecuting ? { backgroundColor: grey[400] } : {}}}
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
                        {this.props.prompt}
                    </Button>
                </DialogActions>
                {!this.state.api.isExecuting ? '' : <CircularProgress style={styles.spinner}/>}
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