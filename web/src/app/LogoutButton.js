import React, { Component } from 'react';
import PropTypes from 'proptypes';

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

    handleLogout = () => {
        return this.props.onLogout();
    }

    handleDialogClose = (result) => {
        this.setState({ confirmDialog: { open: false }});
    }

    render() {
        return (
            <div style={styles.container}>
                <Typography color="inherit" style={styles.caption}>Log Out</Typography>
                <IconButton
                    color="inherit"
                    onClick={this.handleLogoutClick}
                >
                    <ExitToAppIcon/>
                </IconButton>
                <ConfirmDialog
                    title={'Confirm Log Out'}
                    prompt={'Log Out'}
                    open={this.state.confirmDialog.open}
                    onConfirm={this.handleLogout}
                    onClose={this.handleDialogClose}
                    suppressCloseOnConfirm
                >
                    <p>Are you sure you want to log out?</p>
                </ConfirmDialog>
            </div>
        );
    }
}

LogoutButton.propTypes = {
    onLogout: PropTypes.func.isRequired,
}

export default LogoutButton; 