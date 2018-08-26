import React, { Component } from 'react';
import PropTypes from 'proptypes';
import api from '../api';

import IconButton from '@material-ui/core/IconButton';
import Typography from '@material-ui/core/Typography';
import { Person }  from '@material-ui/icons';
import ConfirmDialog from '../shared/ConfirmDialog';
import { Badge } from '@material-ui/core';

const styles = {
    container: {
        marginLeft: 'auto',
        whiteSpace: 'pre',
    },
    caption: {
        marginRight: 10,
        display: 'inline'
    },
    margin: {
        margin: 5,
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
        return api.post('v1/security/logout')
        .then(() => this.props.onLogout());
    }

    handleDialogClose = (result) => {
        this.setState({ confirmDialog: { open: false }});
    }

    render() {
        let { credentials } = this.props;

        return (
            <div style={styles.container}>
                <Typography color="inherit" style={styles.caption}>{credentials.name}</Typography>
                <IconButton
                    color="inherit"
                    onClick={this.handleLogoutClick}
                >
                    {credentials.passwordResetRequired ? 
                        <Badge style={styles.margin} badgeContent={'!'} color="secondary">
                            <Person style={{ fontSize: 30 }}/>
                        </Badge> :
                        <Person style={{ fontSize: 30 }}/>
                    }
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
    credentials: PropTypes.object.isRequired,
    onLogout: PropTypes.func.isRequired,
}

export default LogoutButton; 