/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React, { Component } from 'react';
import Snackbar from '@material-ui/core/Snackbar';
import api from '../api';

const Context = React.createContext();

const initialState = {
	snackbar: {
        message: '',
        open: false,
    }
}

class ContextProvider extends Component {
    state = initialState;

    showMessage = (message) => {
        this.setState({ snackbar: { open: true, message: message }})
    }

    showErrorMessage = (error) => {
        if (error.response && error.response.data) {
            if (error.response.status === 500) {
                this.showMessage(error.response.data.Message);
            } else {
                this.showMessage(error.response.data);
            }
        }
    }

    apiCall = (command, ...args) => {
        return new Promise ((resolve, reject) => 
            command(...args)
                .then(
                    response => {
                        resolve(response);
                    },
                    error => {
                        this.showErrorMessage(error);
                        reject(error);
                    }
                )
        );  
    }

    api = {
        delete: (...args) => { return this.apiCall(api.delete, ...args); },
        get:    (...args) => { return this.apiCall(api.get,    ...args); },
        patch:  (...args) => { return this.apiCall(api.patch,  ...args); },
        post:   (...args) => { return this.apiCall(api.post,   ...args); },
        put:    (...args) => { return this.apiCall(api.put,    ...args); }
    }

    handleSnackbarClose = () => {
        this.setState({ snackbar: { message: '', open: false }});
    }

    render() {
        return (
            <Context.Provider value={{
                showMessage: this.showMessage,
                api: this.api}}
            >
                <div>
                    {this.props.children}
                    <Snackbar
                        anchorOrigin={{ vertical: 'bottom', horizontal: 'center'}}
                        open={this.state.snackbar.open}
                        onClose={this.handleSnackbarClose}
                        autoHideDuration={3000}
                        message={<span id="message-id">{this.state.snackbar.message}</span>}
                    />
                </div>
            </Context.Provider>
        );
    }
}

export const withContext = (Component) => {
    return (props) => {
        return (
            <Context.Consumer>
                {context => (<Component {...props} context={context} />)}
            </Context.Consumer>
        );
    };
}

export default ContextProvider;
