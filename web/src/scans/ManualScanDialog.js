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

class ManualScanDialog extends Component {
    handleCancelClick = () => {
        this.props.onClose();
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
                <DialogTitle>Enter Card Number</DialogTitle>
                <DialogContent>
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