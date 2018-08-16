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
    };

    componentWillMount = () => {
        this.refresh();
    }

    refresh = () => {
        this.setState({ ...this.state, api: { ...this.state.api, isExecuting: true }}, () => {
            api.get('/v1/accounts')
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

    handleDialogClose = (result) => {
        this.setState({ 
            dialog: {
                ...this.state.dialog,
                open: false,
            }
        }, () => {
            if (!result) return;

            if (this.state.dialog.intent === 'add') {
                this.handleAddAccount(result);
            }
            else {
                this.handleEditAccount(result);
            }
        })
    }

    handleAddAccount = (account) => {
        console.log('add', account);
    }

    handleEditAccount = (account) => { 
        console.log('edit', account);
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
                        <AccountList accounts={accounts} onItemClick={this.handleEditClick}/>
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
                    account={dialog.account}
                />
            </ContentWrapper>
        );
    }
}

Accounts.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Accounts); 