/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/
import React, { Component } from 'react';
import PropTypes from 'proptypes';
import api from '../api';

import IconButton from '@material-ui/core/IconButton';
import Typography from '@material-ui/core/Typography';
import { LockOpen, ExitToApp, AccountCircle }  from '@material-ui/icons';
import ConfirmDialog from '../shared/ConfirmDialog';
import { Badge, Menu, MenuItem, Snackbar, ListItemIcon, ListItemText } from '@material-ui/core';
import PasswordResetDialog from '../security/PasswordResetDialog';

const styles = {
    container: {
        marginLeft: 'auto',
        whiteSpace: 'pre',
    },
    caption: {
        marginRight: 5,
        display: 'inline'
    },
    icon: {
        fontSize: 29,
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
        }, () => { 
            if (result) { 
                this.setState({ snackbar: { message: result, open: true }}, () => this.props.onPasswordReset());
            }
        });
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
                            <AccountCircle style={styles.icon}/>
                        </Badge> :
                        <AccountCircle style={styles.icon}/>
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
                            Update Password
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