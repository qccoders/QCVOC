import React, { Component } from 'react';
import PropTypes from 'prop-types';
import api from '../api';

import { withStyles } from '@material-ui/core/styles';

const styles = {
    root: {
        flexGrow: 1,
    },
};

class Accounts extends Component {
    state = { accounts: [] };

    componentWillMount = () => {
        this.refresh();
    }

    refresh = () => {
        api.get('/v1/accounts')
        .then(response => {
            this.setState({ accounts: response.data });
        }, error => {
            console.log('error!', error)
        });
    }

    render() {
        let classes = this.props.classes;
        let { accounts } = this.state;

        return (
            <div className={classes.root}>
                <h1>Accounts</h1>
                <ul>
                    {accounts.map(a => <li key={a.id}>{a.id + ' - ' + a.name + ' (' + a.role + ')'}</li>)}
                </ul>
            </div>
        );
    }
}

Accounts.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Accounts); 