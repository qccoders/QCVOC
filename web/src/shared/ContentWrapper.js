import React from 'react';
import PropTypes from 'prop-types';

import CircularProgress from '@material-ui/core/CircularProgress';
import Error from '@material-ui/icons/Error';

const styles = {
    icon: {
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
    container: {
        padding: 20,
    },
}

const ContentWrapper = (props) => {
    let { isExecuting, isErrored } = props.api;

    return (
        <div style={styles.container}>
            {isErrored && <Error style={{ ...styles.icon, color: 'red' }}/>}
            {!isErrored && isExecuting && <CircularProgress size={20} style={styles.icon}/>}
            {!isErrored && !isExecuting && props.children}
        </div>
    );
}

ContentWrapper.propTypes = {
    api: PropTypes.object.isRequired,
    children: PropTypes.node.isRequired,
};

export default ContentWrapper;