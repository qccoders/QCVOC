import React, { Component } from 'react';
import PropTypes from 'prop-types';

import { getGuid } from '../util';

import { withStyles } from '@material-ui/core/styles';
import { 
    Dialog,
    DialogTitle,
    DialogActions,
    Button,
    DialogContent,
    TextField,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
} from '@material-ui/core';

const styles = {
    dialog: {
    
    },
};

const initialState = {
    account: {
        id: '',
        name: '',
        role: '',
        password: '',
        password2: '',
    },
}

class AccountDialog extends Component {
    state = initialState;

    componentWillReceiveProps = (nextProps) => {
        if (nextProps.open && !this.props.open) {
            this.setState({ 
                ...initialState, 
                account: { 
                    ...initialState.account, 
                    id: getGuid()
                }
            });
        }
    }

    handleChange = (prop, event) => {
        this.setState({ 
            account: {
                ...this.state.account,
                [prop]: event.target.value,
            },
        }, () => {
            console.log(this.state);
        })
    }

    handleCancel = () => {
        this.props.onClose();
    }

    handleOk = () => {
        this.props.onClose(this.state.account);
    }

    render() {
        let { classes, intent, onClose, open, account } = this.props;

        return (
            <Dialog open={open} onClose={onClose} className={classes.dialog}>
                <DialogTitle>{(intent === 'add' ? 'Add' : 'Edit')} Account</DialogTitle>
                <DialogContent>
                    <TextField
                        autoFocus
                        id="name"
                        label="Name"
                        type="text"
                        fullWidth
                        onChange={(event) => this.handleChange('name', event)}
                    />
                    <FormControl 
                        style={{marginTop: 15}}
                        fullWidth
                    >
                        <InputLabel>Role</InputLabel>
                        <Select
                            value={this.state.account.role}
                            onChange={(event) => this.handleChange('role', event)}
                        >
                            <MenuItem value={'User'}>User</MenuItem>
                            <MenuItem value={'Supervisor'}>Supervisor</MenuItem>
                            <MenuItem value={'Administrator'}>Administrator</MenuItem>
                        </Select>
                    </FormControl>
                    <TextField
                        style={{marginTop: 30}}
                        id="password"
                        label="Password"
                        type="password"
                        fullWidth
                        onChange={(event) => this.handleChange('password', event)}
                    />
                    <TextField
                        style={{marginTop: 15}}
                        id="password2"
                        label="Confirm Password"
                        type="password"
                        fullWidth
                        onChange={(event) => this.handleChange('password2', event)}
                    />
                </DialogContent>
                <DialogActions>
                    <Button onClick={this.handleCancel} color="primary">Cancel</Button>
                    <Button onClick={this.handleOk} color="primary">Ok</Button>
                </DialogActions>
            </Dialog>
        );
    }
}

AccountDialog.propTypes = {
    classes: PropTypes.object.isRequired,
    intent: PropTypes.oneOf([ 'add', 'edit' ]).isRequired,
    onClose: PropTypes.func.isRequired,
    open: PropTypes.bool.isRequired,
    account: PropTypes.object,
};

export default withStyles(styles)(AccountDialog); 