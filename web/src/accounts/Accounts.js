import React, { Component } from 'react';
import PropTypes from 'prop-types';

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
        fetch(API_ROOT + '/v1/accounts')
        .then(response => response.json())
        .then(data => {
            this.setState({ accounts: data });
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