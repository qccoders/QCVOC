import React, { Component } from 'react';

import IconButton from '@material-ui/core/IconButton';
import Typography from '@material-ui/core/Typography';
import ExitToAppIcon from '@material-ui/icons/ExitToApp';
import ConfirmDialog from '../shared/ConfirmDialog';

const styles = {
    container: {
        marginLeft: 'auto',
        whiteSpace: 'pre',
    },
    caption: {
        marginRight: 0,
        display: 'inline'
    },
}

const initialState = {
    confirmDialog: {
        open: false,
    },
};

class LogoutButton extends Component {
    state = initialState;

    handleLogoutClick = () => {
        this.setState({ confirmDialog: { open: true }});
    }

    handleDialogClose = (result) => {
        this.setState({ confirmDialog: { open: false }}, () => {
            if (result && result.confirmed) {
                this.props.onLogout();
            }
        });
    }

    render() {
        return (
            <div style={styles.container}>
                <Typography color="inherit" style={styles.caption}>Log Out</Typography>
                <IconButton
                    color="inherit"
                    // onClick={this.props.onLogout}
                    onClick={this.handleLogoutClick}
                >
                    <ExitToAppIcon/>
                </IconButton>
                <ConfirmDialog
                    title={'Confirm Log Out'}
                    open={this.state.confirmDialog.open}
                    onClose={this.handleDialogClose}
                >
                    <p>Are you sure you want to log out?</p>
                </ConfirmDialog>
            </div>
        );
    }
}

export default LogoutButton; 