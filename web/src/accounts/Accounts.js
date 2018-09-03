/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

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
import CircularProgress from '@material-ui/core/CircularProgress';

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
    card: {
        minHeight: 212,
        maxWidth: 800,
        margin: 'auto',
    },
    refreshSpinner: {
        position: 'fixed',
        left: 0,
        right: 0,
        marginLeft: 'auto',
        marginRight: 'auto',
        marginTop: 43,
    },
};

class Accounts extends Component {
    state = { 
        accounts: [],
        loadApi: {
            isExecuting: false,
            isErrored: false,
        },
        refreshApi: {
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
        this.refresh('refreshApi');
    }

    refresh = (apiType) => {
        this.setState({ ...this.state, [apiType]: { ...this.state[apiType], isExecuting: true }}, () => {
            api.get('/v1/security/accounts')
            .then(response => {
                this.setState({ 
                    accounts: response.data,
                    [apiType]: { isExecuting: false, isErrored: false },
                });
            }, error => {
                this.setState({ 
                    ...this.state, 
                    [apiType]: { isExecuting: false, isErrored: true },
                    snackbar: { message: error.response.data, open: true },
                });
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
            if (result) {
                this.setState({ snackbar: { message: result, open: true }}, () => this.refresh('refreshApi'));
            }
        })
    }

    handlePasswordResetDialogClose = (result) => {
        this.setState({
            passwordResetDialog: {
                ...this.state.passwordResetDialog,
                open: false,
            }
        }, () => {
            if (result) {
                this.setState({ snackbar: { message: result, open: true }}, () => this.props.onPasswordReset());
            }
        })
    }

    handleSnackbarClose = () => {
        this.setState({ snackbar: { open: false }});
    }

    render() {
        let { accounts, loadApi, refreshApi, accountDialog, passwordResetDialog } = this.state;
        let { classes } = this.props;

        return (
            <div>
                <ContentWrapper api={loadApi}>
                    <Card className={classes.card}>
                        <CardContent>
                            <Typography gutterBottom variant="headline" component="h2">
                                Accounts
                            </Typography>
                            {refreshApi.isExecuting ? 
                                <CircularProgress size={30} color={'secondary'} className={classes.refreshSpinner}/> :
                                <AccountList 
                                    accounts={accounts} 
                                    onItemClick={this.handleEditClick} 
                                    onItemResetClick={this.handleResetClick}
                                />
                            }
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
                        account={accountDialog.account}
                    />
                    <PasswordResetDialog
                        open={passwordResetDialog.open}
                        account={passwordResetDialog.account}
                        onClose={this.handlePasswordResetDialogClose}
                    />
                </ContentWrapper>
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

Accounts.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Accounts); 