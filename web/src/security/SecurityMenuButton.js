import React, { Component } from 'react';
import PropTypes from 'proptypes';
import api from '../api';

import IconButton from '@material-ui/core/IconButton';
import Typography from '@material-ui/core/Typography';
import { Person, LockOpen, ExitToApp }  from '@material-ui/icons';
import ConfirmDialog from '../shared/ConfirmDialog';
import { Badge, Menu, MenuItem, Divider, ListItemIcon, ListItemText } from '@material-ui/core';

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
    menu: {
        anchorEl: undefined,
        open: false,
    },
};

class SecurityMenuButton extends Component {
    state = initialState;

    handleLogoutClick = () => {
        this.setState({ 
            menu: { open: false },
            confirmDialog: { open: true }
        });
    }

    handleLogout = () => {
        return api.post('v1/security/logout')
        .then(() => this.props.onLogout());
    }

    handleResetPassword = () => {
        
    }

    handleDialogClose = (result) => {
        this.setState({ confirmDialog: { open: false }});
    }

    handleMenuClick = (event) => {
        this.setState({ menu: { anchorEl: event.currentTarget, open: true }});
    }

    handleMenuClose = () => {
        this.setState({ menu: { open: false }});
    }

    handleResetPasswordClick = () => {

    }

    render() {
        let { credentials } = this.props;
        let { menu } = this.state;

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
                            <Badge badgeContent={'!'} color="secondary">
                                <LockOpen/>
                            </Badge>
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

SecurityMenuButton.propTypes = {
    credentials: PropTypes.object.isRequired,
    onLogout: PropTypes.func.isRequired,
    onPasswordReset: PropTypes.func.isRequired,
}

export default SecurityMenuButton; 