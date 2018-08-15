import React, { Component } from 'react';
import PropTypes from 'prop-types';
import api from '../api';

import ContentWrapper from '../shared/ContentWrapper';
import { withStyles } from '@material-ui/core/styles';

const styles = {
};

class Accounts extends Component {
    state = { 
        accounts: [],
        api: {
            isExecuting: false,
            isErrored: false,
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

    render() {
        let { accounts } = this.state;

        return (
            <ContentWrapper api={this.state.api}>
                <h1>Accounts</h1>
                <ul>
                    {accounts.map(a => <li key={a.id}>{a.id + ' - ' + a.name + ' (' + a.role + ')'}</li>)}
                </ul>
            </ContentWrapper>
        );
    }
}

Accounts.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Accounts); 