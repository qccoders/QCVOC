/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React, { Component } from 'react';
import PropTypes from 'prop-types';

const styles = {
    dialog: {
        width: 320,
        marginRight: 'auto',
        marginLeft: 'auto',
        marginTop: 50,
        height: 'fit-content',
    },
    deleteButton: {
        marginRight: 'auto',
    },
    spinner: {
        position: 'fixed',
    },
    verificationSelect: {
        marginTop: 15,
    }
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
    snackbar: {
        message: '',
        open: false,
    },
    confirmDialog: {
        open: false,
    }
}

class ModalDialog extends Component {
    state = initialState;

    componentWillReceiveProps = (nextProps) => {
        if (nextProps.open && !this.props.open) {
            this.setState({
                ...initialState
            });
        }
    }

    handleSaveClick = () => {
        this.validate().then(result => {
            if (result.isValid) {
                if (this.props.intent === 'add') {
                    this.execute(
                        () => api.post('/v1/XXXXXX', xxxxx),
                        'addApi', 
                        'XXXXXX \'' + XXXXXXX + '\' successfully enrolled.'
                    )
                }
                else {
                    if (this.props.veteran.cardNumber 
                        && this.props.veteran.cardNumber !== veteran.cardNumber) {
                        this.setState({ confirmDialog: 'changeCardNumber' });
                    } else {
                        this.execute(
                            () => api.put('/v1/XXXXXX/' + veteran.id, veteran), 
                            'updateApi', 
                            'XXXXXX \'' + XXXXXXX +  '\' successfully updated.'
                        );
                    }
                }
            }
        });
    }

    handleCancelClick = () => {
        this.props.onClose();
    }

    handleDeleteClick = () => {
        this.setState({ });
    }

    handleDeleteConfirmClick = () => {
        return this.execute(
            () => api.delete('/v1/XXXXX/' + this.state.service.id),
            'deleteApi',
            'XXXXX \'' + this.state.service.name + '\' successfully deleted.'
        );
    }


}

