import React, { Component } from 'react';
import PropTypes from 'prop-types';
import api from '../api';

import AccountList from './AccountList';
import ContentWrapper from '../shared/ContentWrapper';
import AccountDialog from './AccountDialog';
import PasswordResetDialog from '../security/PasswordResetDialog';

import { withStyles } from '@material-ui/core/styles';
import { 
    Typography, 
    Card, 
    CardContent,
    Button,
} from '@material-ui/core';
import { Add } from '@material-ui/icons'

import Snackbar from '@material-ui/core/Snackbar';

const styles = {
    fab: {
        margin: 0,
        top: 'auto',
        right: 20,
        bottom: 20,
        left: 'auto',
        position: 'fixed',
        zIndex: 1000
    },
};

class Accounts extends Component {
    state = { 
        accounts: [],
        api: {
            isExecuting: false,
            isErrored: false,
        },
        accountDialog: {
            open: false,
            intent: 'add',
            account: undefined,
        },
        passwordResetDialog: {
            open: false,
            account: undefined,
        },
        snackbar: {
            message: '',
            open: false,
        },
    };

    componentWillMount = () => {
        this.refresh();
    }

    refresh = () => {
        this.setState({ ...this.state, api: { ...this.state.api, isExecuting: true }}, () => {
            api.get('/v1/security/accounts')
            .then(response => {
                this.setState({ 
                    accounts: response.data,
                    api: { isExecuting: false, isErrored: false },
                });
            }, error => {
                this.setState({ ...this.state, api: { isExecuting: false, isErrored: true }});
                console.log(error);
            });
        })
    }

    handleAddClick = () => {
        this.setState({ 
            accountDialog: {
                open: true,
                intent: 'add',
            },
        });
    }

    handleEditClick = (account) => {
        this.setState({
            accountDialog: {
                open: true,
                intent: 'update',
                account: account,
            },
        });
    }

    handleResetClick = (account) => {
        this.setState({
            passwordResetDialog: {
                open: true,
                account: account,
            },
        });
    }

    handleAccountDialogClose = (result) => {
        this.setState({ 
            accountDialog: {
                ...this.state.accountDialog,
                open: false,
            }
        }, () => {
            if (!result) return;
            this.setState({ snackbar: { message: result, open: true }}, () => this.refresh())
        })
    }

    handlePasswordResetDialogClose = (result) => {
        this.setState({
            passwordResetDialog: {
                ...this.state.passwordResetDialog,
                open: false,
            }
        }, () => {
            if (!result) return;
            this.setState({ snackbar: { message: result, open: true }});
        })
    }

    handleSnackbarClose = () => {
        this.setState({ snackbar: { open: false }});
    }

    addAccount = (account) => {
        delete account.password2;
        return api.post('/v1/security/accounts', account);
    }

    updateAccount = (account) => { 
        return api.put('/v1/security/accounts/' + account.id, account);
    }

    deleteAccount = (account) => {
        return api.delete('/v1/security/accounts/' + account.id);
    }

    render() {
        let { accounts, api, accountDialog, passwordResetDialog } = this.state;
        let { classes } = this.props;

        return (
            <ContentWrapper api={api}>
                <Card>
                    <CardContent>
                        <Typography gutterBottom variant="headline" component="h2">
                            Accounts
                        </Typography>
                        <AccountList 
                            accounts={accounts} 
                            onItemClick={this.handleEditClick} 
                            onItemResetClick={this.handleResetClick}
                        />
                    </CardContent>
                </Card>
                <Button 
                    variant="fab" 
                    color="secondary" 
                    className={classes.fab}
                    onClick={this.handleAddClick}
                >
                    <Add/>
                </Button>
                <AccountDialog
                    open={accountDialog.open}
                    intent={accountDialog.intent} 
                    onClose={this.handleAccountDialogClose}
                    addAccount={this.addAccount}
                    updateAccount={this.updateAccount}
                    deleteAccount={this.deleteAccount}
                    account={accountDialog.account}
                />
                <PasswordResetDialog
                    open={passwordResetDialog.open}
                    account={passwordResetDialog.account}
                    onClose={this.handlePasswordResetDialogClose}
                    updateAccount={this.updateAccount}
                />
                <Snackbar
                    anchorOrigin={{ vertical: 'bottom', horizontal: 'center'}}
                    open={this.state.snackbar.open}
                    onClose={this.handleSnackbarClose}
                    autoHideDuration={3000}
                    message={<span id="message-id">{this.state.snackbar.message}</span>}
                />
            </ContentWrapper>
        );
    }
}

Accounts.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Accounts); 