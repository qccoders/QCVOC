import React, { Component } from 'react';
import Snackbar from '@material-ui/core/Snackbar';

const ServiceContext = React.createContext();

const initialState = {
	snackbar: {
        message: '',
        open: false,
    }
}

class ServiceProvider extends Component {
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

    handleSnackbarClose = () => {
        this.setState({ snackbar: { message: '', open: false }});
    }

    render() {
        return (
            <ServiceContext.Provider value={{
                showMessage: this.showMessage,
                apiCall: this.apiCall}}>
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
	        </ServiceContext.Provider>
        );
    }
}

export const withContext = (Component) => {
    return (props) => {
        return (
            <ServiceContext.Consumer>
                {context => (<Component {...props} context={context} />)}
            </ServiceContext.Consumer>
        );
    };
}

export default ServiceProvider;
