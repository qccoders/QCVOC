/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React, { Component } from 'react';
import PropTypes from 'prop-types';
import moment from 'moment';

import { withStyles } from '@material-ui/core/styles';
import { 
    Dialog,
    DialogTitle,
    DialogActions,
    Button,
    DialogContent,
    TextField,
    CircularProgress,
} from '@material-ui/core';

import DateTimePicker from 'material-ui-pickers/DateTimePicker';

import { withContext } from '../shared/ContextProvider';
import ConfirmDialog from '../shared/ConfirmDialog';

const styles = {
    dialog: {
        width: 320,
        marginRight: 'auto',
        marginLeft: 'auto',
        marginTop: 50,
        height: 'fit-content',
    },
    leftButton: {
        marginRight: 'auto',
    },
    spinner: {
        position: 'fixed',
    },
};

const initialState = {
    addApi: {
        isExecuting: false,
        isErrored: false,
    },
    deleteApi: {
        isExecuting: false,
        isErrored: false,
    },
    updateApi: {
        isExecuting: false,
        isErrored: false,
    },
    event: {
        name: '',
        startDate: null,
        endDate: null,
    },
    validation: {
        name: undefined,
        startDate: undefined,
        endDate: undefined,
    },
    confirmDialog: {
        open: false,
    },
};

class EventDialog extends Component {
    state = initialState;

    componentWillReceiveProps = (nextProps) => {
        if (nextProps.open && !this.props.open) {
            this.setState({ 
                ...initialState, 
                event: nextProps.event ? nextProps.event : { 
                    ...initialState.event, 
                },
                validation: initialState.validation,
            });
        }
    }

    handleSaveClick = () => {
        let event = { 
            ...this.state.event,
            startDate: moment(this.state.event.startDate).format(),
            endDate: moment(this.state.event.endDate).format(),
        };

        this.validate().then(result => {
            if (result.isValid) {
                if (this.props.intent === 'add') {
                    this.execute(
                        () => this.props.context.api.post('/v1/events', event),
                        'addApi',
                        'Event \'' + event.name + '\' successfully created.'
                    );
                }
                else {
                    this.execute(
                        () => this.props.context.api.put('/v1/events/' + event.id, event),
                        'updateApi',
                        'Event \'' + event.name + '\' successfully updated.'
                    );
                }
            }
        });
    }

    handleCancelClick = () => {
        this.props.onClose();
    }

    handleDeleteClick = () => {
        this.setState({ confirmDialog: { open: true }});
    }

    handleDailyEventClick = () => {
        let start = moment().startOf('day').add(8, 'hours');
        let end = moment().startOf('day').add(8, 'hours').add(7, 'hours');

        this.setState({ 
            event: {
                name: 'Daily Event for ' + start.format('M/DD/YY'),
                startDate: start.format(),
                endDate: end.format(),
            },
            validation: { ...initialState.validation },
        });
    }

    handleDeleteConfirmClick = () => {
        return this.execute(
            () => this.props.context.api.delete('/v1/events/' + this.state.event.id),
            'deleteApi',
            'Event \'' + this.state.event.name + '\' successfully deleted.'
        );
    }

    handleChange = (field, value) => {
        this.setState({ 
            event: {
                ...this.state.event,
                [field]: value,
            },
            validation: {
                ...this.state.validation,
                [field]: undefined,
            },
        });
    }

    handleDialogClose = (result) => {
        this.setState({ confirmDialog: { open: false }});
    }

    execute = (action, api, successMessage) => {
        return new Promise((resolve, reject) => {
            this.setState({ [api]: { isExecuting: true }}, () => {
                action()
                .then(response => {
                    this.setState({
                        [api]: { isExecuting: false, isErrored: false },
                    }, () => {
                        this.props.onClose(successMessage);
                        resolve(response);
                    });
                }, error => {
                    var body = error && error.response && error.response.data ? error.response.data : error;

                    if (typeof(body) !== 'string') {
                        var keys = Object.keys(body);
    
                        if (keys.length > 0) {
                            body = body[keys[0]];
                        }
                    }

                    this.setState({ 
                        [api]: { isExecuting: false, isErrored: true },
                    }, () => reject(error));
                });
            });
        });
    }

    validate = () => {
        let { name, startDate, endDate } = this.state.event;
        let result = { ...initialState.validation };

        if (name === '') result.name = 'The Name field is required.';
        if (startDate === null) result.startDate = 'The Start Date field is required.';

        let start = new Date(startDate);
        if (!(start instanceof Date) || isNaN(start)) {
            result.startDate = 'The specified Start Date is not a valid date.';
        }

        if (endDate === null) result.endDate = 'The End Date field is required.';

        let end = new Date(endDate);
        if (!(end instanceof Date) || isNaN(end)) {
            result.endDate = 'The specified End Date is not a valid date.';
        }

        if (start > end) {
            result.startDate = result.endDate = 'Start Date must come before End Date.';
        }

        return new Promise(resolve => {
            this.setState({ validation: result }, () => {
                result.isValid = JSON.stringify(result) === JSON.stringify(initialState.validation);
                resolve(result);
            });
        });
    }

    render() {
        let { classes, intent, open } = this.props;
        let { name, startDate, endDate } = this.state.event;
        let validation = this.state.validation;

        let adding = this.state.addApi.isExecuting;
        let updating = this.state.updateApi.isExecuting;
        let deleting = this.state.deleteApi.isExecuting;
        
        let executing = adding || updating || deleting;
        
        let dim = executing ? { opacity: 0.5 } : undefined;

        return (
            <Dialog 
                open={open}
                onClose={this.handleCancelClick}
                PaperProps={{ className: classes.dialog }}
                scroll={'body'}
            >
                <DialogTitle style={dim}>{(intent === 'add' ? 'Create' : 'Update')} Event</DialogTitle>
                <DialogContent>
                    <TextField
                        autoFocus
                        id="name"
                        label="Name"
                        value={name}
                        type="text"
                        fullWidth
                        onChange={(event) => this.handleChange('name', event.target.value)}
                        helperText={validation.name}
                        error={validation.name !== undefined}
                        disabled={executing}
                        margin={'normal'}
                    /> 
                    <DateTimePicker
                        value={startDate}
                        fullWidth
                        label="Start Date"
                        helperText={validation.startDate}
                        error={validation.startDate !== undefined}
                        disabled={executing}
                        margin={'normal'}
                        onChange={(event) => this.handleChange('startDate', event.format())}
                    />
                    <DateTimePicker
                        value={endDate}
                        fullWidth
                        label="End Date"
                        helperText={validation.endDate}
                        error={validation.endDate !== undefined}
                        disabled={executing}
                        margin={'normal'}
                        onChange={(event) => this.handleChange('endDate', event.format())}
                    />
                </DialogContent>
                <DialogActions>
                    {intent === 'update' && 
                        <Button 
                            onClick={this.handleDeleteClick} 
                            color="primary" 
                            className={classes.leftButton}
                            disabled={executing}
                        >
                            {deleting && <CircularProgress size={20} style={styles.spinner}/>}
                            Delete
                        </Button>
                    }
                    {intent === 'add' && 
                        <Button 
                            onClick={this.handleDailyEventClick} 
                            color="primary" 
                            className={classes.leftButton}
                            disabled={executing}
                        >
                            Daily Event
                        </Button>
                    }
                    <Button 
                        onClick={this.handleCancelClick}
                        color="primary"
                        disabled={executing}
                    >
                        Cancel
                    </Button>
                    <Button 
                        onClick={this.handleSaveClick} 
                        color="primary"
                        disabled={executing}
                    >
                        {(adding || updating) && <CircularProgress size={20} style={styles.spinner}/>}
                        {intent === 'add' ? 'Add' : 'Update'}
                    </Button>
                </DialogActions>
                <ConfirmDialog
                    title={'Confirm Event Deletion'}
                    prompt={'Delete'}
                    open={this.state.confirmDialog.open}
                    onConfirm={this.handleDeleteConfirmClick}
                    onClose={this.handleDialogClose}
                    suppressCloseOnConfirm
                >
                    <p>Are you sure you want to delete Event '{name + ' starting ' + moment(startDate).format('dddd, MMMM Do [at] LT')}?</p>
                </ConfirmDialog>
            </Dialog>
        );
    }
}

EventDialog.propTypes = {
    classes: PropTypes.object.isRequired,
    intent: PropTypes.oneOf([ 'add', 'update' ]).isRequired,
    open: PropTypes.bool.isRequired,
    onClose: PropTypes.func.isRequired,
    event: PropTypes.object,
};

export default withStyles(styles)(withContext(EventDialog)); 