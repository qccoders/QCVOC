/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

import React, { Component } from "react";
import PropTypes from "prop-types";

import { withStyles } from "@material-ui/core/styles";
import {
  Card,
  CardContent,
  Button,
  Typography,
} from "@material-ui/core";

import { withContext } from "../shared/ContextProvider";
import api from '../api';

const styles = {
  card: {
    minHeight: 273,
    maxWidth: 800,
    margin: "auto",
  },
  button: {
    display: 'block',
    margin: "auto",
    marginTop: 30,
    width: "100%",
    maxWidth: 500,
  }
};

class VeteranReport extends Component {
  getReport = () => {
    api.get('/v1/reports/veteran?format=csv')
    .then (response => {
      const now = new Date();
      const filename = `qcvoc-veterans-${now.getMonth()}-${now.getDay()}-${now.getFullYear()}.csv`;

      const blob = new Blob([response.data], { type: response.headers['content-type'], encoding: 'UTF-8' });

      const link = document.createElement('a');
      link.href = window.URL.createObjectURL(blob);
      link.download = filename;

      link.click();
    });
  };

  render() {
    let classes = this.props.classes;

    return (
      <div className={classes.root}>
          <Card className={classes.card}>
            <CardContent>
              <Typography gutterBottom variant="h5">
                Veteran Report
              </Typography>
              <Button
                className={classes.button} 
                variant="contained" 
                color="primary" 
                onClick={this.getReport}
              >
                Download CSV
              </Button>
            </CardContent>
          </Card>
      </div>
    );
  }
}

VeteranReport.propTypes = {
  classes: PropTypes.object.isRequired
};

export default withStyles(styles)(withContext(VeteranReport));
