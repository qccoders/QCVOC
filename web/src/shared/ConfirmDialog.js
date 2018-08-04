import React, { Component } from 'react';
import PropTypes from 'prop-types';

import Button from '@material-ui/core/Button';
import DialogTitle from '@material-ui/core/DialogTitle';
import DialogContent from '@material-ui/core/DialogContent';
import DialogActions from '@material-ui/core/DialogActions';
import Dialog from '@material-ui/core/Dialog';

const initialState = {
    api: {
        isExecuting: false,
        isErrored: false,
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
                aria-labelledby="confirmation-dialog-title"
                {...additionalProps}
            >
                <DialogTitle id="confirmation-dialog-title">{this.props.title}</DialogTitle>
                <DialogContent>
                    {this.props.children}
                </DialogContent>
                <DialogActions>
                    <Button onClick={this.handleCancelClick} color="primary">
                        Cancel
                    </Button>
                    <Button onClick={this.handleConfirmClick} color="primary">
                        {this.props.prompt}
                    </Button>
                </DialogActions>
            </Dialog>
        );
    }
}

ConfirmDialog.propTypes = {
    title: PropTypes.string,
    onClose: PropTypes.func,
    onConfirm: PropTypes.func,
    children: PropTypes.object,
    prompt: PropTypes.string,
    suppressCloseOnConfirm: PropTypes.bool,
};

export default ConfirmDialog;