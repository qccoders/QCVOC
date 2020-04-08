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
  Typography,
  CircularProgress
} from "@material-ui/core";

import { withContext } from "../shared/ContextProvider";
import ContentWrapper from "../shared/ContentWrapper";
import EventReportTable from "./EventReportTable";

const styles = {
  card: {
    minHeight: 273,
    maxWidth: 800,
    margin: "auto"
  },
  refreshSpinner: {
    position: "fixed",
    left: 0,
    right: 0,
    marginLeft: "auto",
    marginRight: "auto",
    marginTop: 80
  }
};

class EventReport extends Component {
  state = {
    events: [],
    loadApi: {
      isExecuting: false,
      isErrored: false
    },
    refreshApi: {
      isExecuting: false,
      isErrored: false
    }
  };

  componentWillMount = () => {
    this.refresh("refreshApi");
  };

  refresh = apiType => {
    this.setState(
      { [apiType]: { ...this.state[apiType], isExecuting: true } },
      () => {
        this.props.context.api
          .get("/v1/reports/event/master?startTime=1/1/2019&endTime=1/1/2021")
          .then(
            response => {
              this.setState({
                events: response.data,
                [apiType]: { isExecuting: false, isErrored: false }
              });
            },
            error => {
              this.setState({
                [apiType]: { isExecuting: false, isErrored: true }
              });
            }
          );
      }
    );
  };

  render() {
    let classes = this.props.classes;
    let { events, loadApi, refreshApi } = this.state;

    events = events.map(e => ({
      ...e,
      startDate: new Date(e.startDate).getTime(),
      endDate: new Date(e.endDate).getTime()
    }));

    //let now = new Date().getTime();

    return (
      <div className={classes.root}>
        <ContentWrapper api={loadApi}>
          <Card className={classes.card}>
            <CardContent>
              <Typography gutterBottom variant="h5">
                Event Report
              </Typography>
              {refreshApi.isExecuting ? (
                <CircularProgress
                  size={30}
                  color={"secondary"}
                  className={classes.refreshSpinner}
                />
              ) : (
                <EventReportTable data={events || []} />
              )}
            </CardContent>
          </Card>
        </ContentWrapper>
      </div>
    );
  }
}

EventReport.propTypes = {
  classes: PropTypes.object.isRequired
};

export default withStyles(styles)(withContext(EventReport));
