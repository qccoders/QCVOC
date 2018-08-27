import React, { Component } from 'react';
import PropTypes from 'proptypes';
import api from '../api';

import IconButton from '@material-ui/core/IconButton';
import Typography from '@material-ui/core/Typography';
import { Person, LockOpen, ExitToApp }  from '@material-ui/icons';
import ConfirmDialog from '../shared/ConfirmDialog';
import { Badge, Menu, MenuItem, Snackbar, ListItemIcon, ListItemText } from '@material-ui/core';
import PasswordResetDialog from '../security/PasswordResetDialog';

const styles = {
    container: {
        marginLeft: 'auto',
        whiteSpace: 'pre',
    },
    caption: {
        marginRight: 10,
        display: 'inline'
    },
    icon: {
        fontSize: 30,
    },
    menu: {
        marginTop: 40,
    },
}

const initialState = {
    confirmDialog: {
        open: false,
    },
    passwordResetDialog: {
        open: false,
        account: undefined,
    },
    menu: {
        anchorEl: undefined,
        open: false,
    },
    snackbar: {
        open: false,
        message: undefined,
    },
};

class SecurityMenu extends Component {
    state = initialState;

    logout = () => {
        return api.post('v1/security/logout')
        .then(() => this.props.onLogout());
    }

    resetPassword = (account) => {
        return api.put('v1/security/accounts/' + account.id, account)
        .then(() => this.props.onPasswordReset());
    }

    handleMenuClick = (event) => {
        this.setState({ menu: { anchorEl: event.currentTarget, open: true }});
    }

    handleLogoutClick = () => {
        this.setState({ 
            menu: { open: false },
            confirmDialog: { open: true }
        });
    }

    handleResetPasswordClick = () => {
        this.setState({ 
            menu: { open: false },
            passwordResetDialog: { 
                open: true, 
                account: { id: this.props.credentials.id, name: this.props.credentials.name }
            }
        });
    }

    handleMenuClose = () => {
        this.setState({ menu: { open: false }});
    }

    handleConfirmDialogClose = (result) => {
        this.setState({ confirmDialog: { open: false }});
    }

    handleSnackbarClose = () => {
        this.setState({ snackbar: { open: false }});
    }

    handlePasswordResetDialogClose = (result) => {
        this.setState({
            passwordResetDialog: { open: false },
        });

        if (result) { 
            this.setState({ snackbar: { message: result, open: true }});
        }
    }

    render() {
        let { credentials } = this.props;
        let { menu, passwordResetDialog } = this.state;

        return (
            <div style={styles.container}>
                <Typography color="inherit" style={styles.caption}>{credentials.name}</Typography>
                <IconButton
                    color="inherit"
                    onClick={this.handleMenuClick}
                >
                    {credentials.passwordResetRequired ? 
                        <Badge badgeContent={'!'} color="secondary">
                            <Person style={styles.icon}/>
                        </Badge> :
                        <Person style={styles.icon}/>
                    }
                </IconButton>
                <Menu
                    open={menu.open}
                    anchorEl={menu.anchorEl}
                    onClose={this.handleMenuClose}
                    style={styles.menu}
                >
                    <MenuItem onClick={this.handleResetPasswordClick}>
                        <ListItemIcon>
                            {credentials.passwordResetRequired ? 
                                <Badge badgeContent={'!'} color="secondary">
                                    <LockOpen/>
                                </Badge> :
                                <LockOpen/>
                            }
                        </ListItemIcon>
                        <ListItemText>
                            Reset Password
                        </ListItemText>
                    </MenuItem>
                    <MenuItem onClick={this.handleLogoutClick}>
                        <ListItemIcon>
                            <ExitToApp/>
                        </ListItemIcon>
                        <ListItemText>
                            Log Out
                        </ListItemText>
                    </MenuItem>
                </Menu>
                <ConfirmDialog
                    title={'Confirm Log Out'}
                    prompt={'Log Out'}
                    open={this.state.confirmDialog.open}
                    onConfirm={this.logout}
                    onClose={this.handleConfirmDialogClose}
                    suppressCloseOnConfirm
                >
                    <p>Are you sure you want to log out?</p>
                </ConfirmDialog>
                <PasswordResetDialog
                    open={passwordResetDialog.open}
                    account={passwordResetDialog.account}
                    onClose={this.handlePasswordResetDialogClose}
                    onReset={this.resetPassword}
                />
                <Snackbar
                    anchorOrigin={{ vertical: 'bottom', horizontal: 'center'}}
                    open={this.state.snackbar.open}
                    onClose={this.handleSnackbarClose}
                    autoHideDuration={3000}
                    message={<span id="message-id">{this.state.snackbar.message}</span>}
                />
            </div>
        );
    }
}

SecurityMenu.propTypes = {
    credentials: PropTypes.object.isRequired,
    onLogout: PropTypes.func.isRequired,
    onPasswordReset: PropTypes.func.isRequired,
}

export default SecurityMenu; 