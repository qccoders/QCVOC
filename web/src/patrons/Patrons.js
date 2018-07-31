import React, { Component } from 'react';
import PropTypes from 'prop-types';

import { withStyles } from '@material-ui/core/styles';

const styles = {
    root: {
        flexGrow: 1,
    },
};

class Patrons extends Component {
    render() {
        let classes = this.props.classes;

        return (
            <div className={classes.root}>
                Patrons
            </div>
        );
    }
}

Patrons.propTypes = {
    classes: PropTypes.object.isRequired,
};

export default withStyles(styles)(Patrons); 