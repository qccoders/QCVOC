import React from 'react';

import IconButton from '@material-ui/core/IconButton';
import Typography from '@material-ui/core/Typography';
import ExitToAppIcon from '@material-ui/icons/ExitToApp';

const styles = {
    container: {
        marginLeft: 'auto',
        whiteSpace: 'pre',
    },
    caption: {
        marginRight: 10,
    },
}

const LogoutButton = (props) => {
    return (
        <div style={styles.container}>
            <IconButton
                color="inherit"
                onClick={props.onLogout}
            >
                <Typography color="inherit" style={styles.caption}>Log Out</Typography>
                <ExitToAppIcon/>
            </IconButton>
        </div>
    );
}

export default LogoutButton; 