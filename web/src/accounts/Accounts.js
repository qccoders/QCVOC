/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React, { Component } from 'react';
import PropTypes from 'prop-types';

import { withStyles } from '@material-ui/core/styles';
import { 
    Typography, 
    Card, 
    CardContent,
    Fab,
    CircularProgress,
} from '@material-ui/core';
import { Add } from '@material-ui/icons';

import { withContext } from '../shared/ContextProvider';
import AccountList from './AccountList';
import ContentWrapper from '../shared/ContentWrapper';
import AccountDialog from './AccountDialog';
import PasswordResetDialog from '../security/PasswordResetDialog';

const styles = {
    fab: {
        margin: 0,
        top: 'auto',
        right: 20,
        bottom: 20,
        left: 'auto',
        position: 'fixed',
        zIndex: 1000,
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
        marginTop: 68,
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
    };

    componentWillMount = () => {
        this.refresh('refreshApi');
    }

    refresh = (apiType) => {
        this.setState({ ...this.state, [apiType]: { ...this.state[apiType], isExecuting: true }}, () => {
            this.props.context.api.get('/v1/security/accounts')
            .then(response => {
                this.setState({ 
                    accounts: response.data,
                    [apiType]: { isExecuting: false, isErrored: false },
                });
            }, error => {
                this.setState({ 
                    ...this.state, 
                    [apiType]: { isExecuting: false, isErrored: true },
                });
            });
        });
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
            },
        }, () => {
            if (result) {
                this.props.context.showMessage(result);
                this.refresh('refreshApi');
            }
        });
    }

    handlePasswordResetDialogClose = (result) => {
        this.setState({
            passwordResetDialog: {
                ...this.state.passwordResetDialog,
                open: false,
            },
        }, () => {
            if (result) {
                this.props.context.showMessage(result);
                this.props.onPasswordReset();
            }
        });
    }

    render() {
        let { accounts, loadApi, refreshApi, accountDialog, passwordResetDialog } = this.state;
        let { classes } = this.props;

        return (
            <div>
                <ContentWrapper api={loadApi}>
                    <Card className={classes.card}>
                        <CardContent>
                            <Typography gutterBottom variant="h5">
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
                    <Fab
                        color="secondary" 
                        className={classes.fab}
                        onClick={this.handleAddClick}
                    >
                        <Add/>
                    </Fab>
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
            </div>            
        );
    }
}

Accounts.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(withContext(Accounts)); 