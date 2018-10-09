/*
    Copyright (c) QC Coders (JP Dillingham, Nick Acosta, Will Burklund, et. al.). All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/
import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import MuiPickersUtilsProvider from 'material-ui-pickers/utils/MuiPickersUtilsProvider';
import MomentUtils from 'material-ui-pickers/utils/moment-utils';
import ServiceProvider from './shared/ServiceContext';

import './index.css';
import App from './app/App';

ReactDOM.render(
    <BrowserRouter>
        <MuiPickersUtilsProvider utils={MomentUtils}>
            <ServiceProvider>
                <App />
            </ServiceProvider>
        </MuiPickersUtilsProvider>
    </BrowserRouter>,
    document.getElementById('root')
);
