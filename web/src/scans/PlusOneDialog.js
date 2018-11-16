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
    plusOne: undefined,
};

class PlusOneDialog extends Component {
    state = initialState;

    componentWillReceiveProps = (nextProps) => {
        if (nextProps.open && !this.props.open) {
            this.setState({ 
                ...initialState,
            });
        }
    }

    handleCancelClick = () => {
        this.props.onClose(undefined);
    }

    handleSelectClick = (plusOne) => {
        this.props.onClose(plusOne);
    }

    handleChange = (event) => {
        this.setState({ 
            cardNumber: event.target.value,
            validation: { cardNumber: undefined },
        });
    }
    
    render() {
        let { classes, open } = this.props;
        
        return (
            <Dialog 
                open={open}
                onClose={this.handleCancelClick}
                PaperProps={{ className: classes.dialog }}
                scroll={'body'}                
            >
                <DialogTitle>Plus One?</DialogTitle>
                <DialogContent>
                    <Button fullWidth style={{ marginBottom: 20 }} onClick={() => this.handleSelectClick(false)}>Veteran Only</Button>
                    <Button fullWidth onClick={() => this.handleSelectClick(true)}>Veteran +1</Button>
                </DialogContent>
                <DialogActions>
                    <Button 
                        onClick={this.handleCancelClick}
                        color="primary"
                    >
                        Cancel
                    </Button>
                </DialogActions>
            </Dialog>
        );
    }
}

PlusOneDialog.propTypes = {
    classes: PropTypes.object.isRequired,
    open: PropTypes.bool.isRequired,
};

export default withStyles(styles)(PlusOneDialog); 