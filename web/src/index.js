/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/
import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import './index.css';

import MuiPickersUtilsProvider from 'material-ui-pickers/MuiPickersUtilsProvider';
import MomentUtils from '@date-io/moment';

import App from './app/App';
import ContextProvider from './shared/ContextProvider';

window.__MUI_USE_NEXT_TYPOGRAPHY_VARIANTS__ = true;

ReactDOM.render(
    <BrowserRouter>
        <MuiPickersUtilsProvider utils={MomentUtils}>
            <ContextProvider>
                <App />
            </ContextProvider>
        </MuiPickersUtilsProvider>
    </BrowserRouter>,
    document.getElementById('root')
);
