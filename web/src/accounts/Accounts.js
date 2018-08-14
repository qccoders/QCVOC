import React, { Component } from 'react';
import PropTypes from 'prop-types';
import api from '../api';

import { API_ROOT } from '../constants';

import { withStyles } from '@material-ui/core/styles';

const styles = {
    root: {
        flexGrow: 1,
    },
};

class Accounts extends Component {
    state = { accounts: [] };

    componentWillMount = () => {
        api.get('/v1/accounts')
        .then(data => {
            console.log(data);
            this.setState({ accounts: data });
        }, error => {
            console.log('error!', error)
        });
    }

    render() {
        let classes = this.props.classes;

        console.log(this.state.accounts);

        return (
            <div className={classes.root}>
                Accounts
            </div>
        );
    }
}

Accounts.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Accounts); 