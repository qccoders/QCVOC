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
    
    componentWillReceiveProps(nextProps) {
        if (nextProps.value !== this.props.value) {
            this.setState({ value: nextProps.value });
        }
    }

    handleCancel = () => {
        this.props.onClose({ confirmed: false });
    };

    handleOk = () => {
        this.props.onClose({ confirmed: true });
    };

    render() {
        return (
            <Dialog
                onEntering={this.handleEntering}
                aria-labelledby="confirmation-dialog-title"
                {...this.props}
            >
                <DialogTitle id="confirmation-dialog-title">{this.props.title}</DialogTitle>
                <DialogContent>
                    {this.props.children}
                </DialogContent>
                <DialogActions>
                    <Button onClick={this.handleCancel} color="primary">
                        Cancel
                    </Button>
                    <Button onClick={this.handleOk} color="primary">
                        Ok
                    </Button>
                </DialogActions>
            </Dialog>
        );
    }
}

ConfirmDialog.propTypes = {
    title: PropTypes.string,
    onClose: PropTypes.func,
    value: PropTypes.string,
    children: PropTypes.object,
};

export default ConfirmDialog;