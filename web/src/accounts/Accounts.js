import React, { Component } from 'react';
import PropTypes from 'prop-types';
import api from '../api';

import AccountList from './AccountList';
import ContentWrapper from '../shared/ContentWrapper';
import AccountDialog from './AccountDialog';

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
        dialog: {
            open: false,
            intent: 'add',
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
            dialog: {
                open: true,
                intent: 'add',
            },
        });
    }

    handleEditClick = (account) => {
        this.setState({
            dialog: {
                open: true,
                intent: 'edit',
                account: account,
            },
        });
    }

    handleResetClick = (account) => {
        this.editAccount(account);
    }

    handleDialogClose = (result) => {
        console.log(result)
        this.setState({ 
            dialog: {
                ...this.state.dialog,
                open: false,
            }
        }, () => {
            if (!result) return;
            this.setState({ snackbar: { message: result, open: true }}, () => this.refresh())
            //this.refresh();
        })
    }

    handleSnackbarClose = () => {
        this.setState({ snackbar: { open: false }});
    }

    addAccount = (account) => {
        delete account.password2;

        return new Promise((resolve, reject) => {
            api.post('/v1/security/accounts', account)
            .then(response => {
                resolve(response);
            })
            .catch(error => {
                reject(error);
            })
        })
    }

    editAccount = (account) => { 
        console.log('edit', account);
    }

    deleteAccount = (id) => {
        return new Promise((resolve, reject) => {
            api.delete('/v1/security/accounts/' + id)
            .then(response => {
                resolve(response);
            })
            .catch(error => {
                reject(error);
            })
        })
    }

    render() {
        let { accounts, api, dialog } = this.state;
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
                    open={dialog.open}
                    intent={dialog.intent} 
                    onClose={this.handleDialogClose}
                    addAccount={this.addAccount}
                    deleteAccount={this.deleteAccount}
                    account={dialog.account}
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