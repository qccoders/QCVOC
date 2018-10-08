/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React, { Component } from 'react';
import PropTypes from 'prop-types';
import { withStyles } from '@material-ui/core/styles';
import { 
    Dialog,
    DialogTitle,
    DialogActions,
    Button,
    DialogContent,
    TextField,
} from '@material-ui/core';

const styles = {
    dialog: {
        width: 320,
        marginLeft: 'auto',
        marginRight: 'auto',
        marginTop: 50,
        height: 'fit-content',
    },
};

const initialState = {
    cardNumber: undefined,
    validation: {
        cardNumber: undefined,
    },
};

class ManualScanDialog extends Component {
    state = initialState;

    componentWillReceiveProps = (nextProps) => {
        if (nextProps.open && !this.props.open) {
            this.setState({ 
                ...initialState
            });
        }
    }

    handleCancelClick = () => {
        this.props.onClose(undefined);
    }

    handleEnterClick = () => {
        this.validate().then(result => {
            if (result.isValid) {
                this.props.onClose(this.state.cardNumber);
            }
        });
    }

    handleChange = (event) => {
        this.setState({ 
            cardNumber: event.target.value,
            validation: { cardNumber: undefined },
        });
    }

    validate = () => {
        let result = { ...initialState.validation };

        if (this.state.cardNumber === undefined) {
            result.cardNumber = 'A Card Number is required.';
        }

        if (isNaN(this.state.cardNumber)) {
            result.cardNumber = 'The Card Number must contain only numbers.';
        }

        return new Promise(resolve => {
            this.setState({ validation: result }, () => {
                result.isValid = JSON.stringify(result) === JSON.stringify(initialState.validation);
                resolve(result);
            });                
        })
    }
    
    render() {
        let { classes, open } = this.props;
        let { validation } = this.state;
        
        return (
            <Dialog 
                open={open}
                onClose={this.handleCancelClick}
                PaperProps={{ className: classes.dialog }}
                scroll={'body'}                
            >
                <DialogTitle>Enter Card Number</DialogTitle>
                <DialogContent>
                    <TextField
                        style={{marginTop: 0}}
                        id="cardNumber"
                        label="Card Number"
                        type="numeric"
                        error={validation.cardNumber !== undefined}
                        helperText={validation.cardNumber}
                        fullWidth
                        onChange={(event) => this.handleChange(event)}
                    />
                </DialogContent>
                <DialogActions>
                    <Button 
                        onClick={this.handleCancelClick}
                        color="primary"
                    >
                        Cancel
                    </Button>
                    <Button 
                        onClick={this.handleEnterClick}
                        color="primary"
                    >
                        Enter
                    </Button>
                </DialogActions>
            </Dialog>
        );
    }
}

ManualScanDialog.propTypes = {
    classes: PropTypes.object.isRequired,
    open: PropTypes.bool.isRequired,
};

export default withStyles(styles)(ManualScanDialog); 