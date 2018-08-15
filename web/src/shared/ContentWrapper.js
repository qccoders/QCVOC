import React, { Component } from 'react';
import PropTypes from 'prop-types';

import CircularProgress from '@material-ui/core/CircularProgress';

const styles = {
    spinner: {
        position: 'fixed',
        width: 45,
        height: 45,
        left: 0,
        right: 0,
        top: 0,
        bottom: 0,
        marginLeft: 'auto',
        marginRight: 'auto',
        marginTop: 'auto',
        marginBottom: 'auto',
    },
}

class ContentWrapper extends Component {
    render() {
        let { isExecuting, isErrored } = this.props.api;

        isExecuting = true;
        return (
            <div>
                {!isErrored && isExecuting && <CircularProgress size={20} style={styles.spinner}/>}
                {!isErrored && !isExecuting && this.props.children}
                {isErrored && 'error'}
            </div>
        );
    }
}

ContentWrapper.propTypes = {
    api: PropTypes.object.isRequired,
    children: PropTypes.node.isRequired,
};

export default ContentWrapper;