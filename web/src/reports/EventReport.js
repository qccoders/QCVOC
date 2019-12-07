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
  CircularProgress,
  Fab
} from "@material-ui/core";
import { Add } from "@material-ui/icons";

import { withContext } from "../shared/ContextProvider";
import { userCanView } from "../util";
import ContentWrapper from "../shared/ContentWrapper";
import ReportTable from "./ReportTable";

const styles = {
  fab: {
    margin: 0,
    top: "auto",
    right: 20,
    bottom: 20,
    left: "auto",
    position: "fixed",
    zIndex: 1000
  },
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

const showCount = 3;

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
    },
    eventDialog: {
      open: false,
      intent: "add",
      event: undefined
    },
    show: showCount
  };

  componentWillMount = () => {
    this.refresh("refreshApi");
  };

  handleEditClick = event => {
    this.setState({
      eventDialog: {
        open: true,
        intent: "update",
        event: event
      }
    });
  };

  handleAddClick = () => {
    this.setState({
      eventDialog: {
        open: true,
        intent: "add",
        event: undefined
      }
    });
  };

  handleShowMoreClick = () => {
    this.setState({ show: this.state.show + showCount });
  };

  handleEventDialogClose = result => {
    this.setState(
      {
        eventDialog: {
          ...this.state.eventDialog,
          open: false
        }
      },
      () => {
        if (!result) return;
        this.props.context.showMessage(result);
        this.refresh("refreshApi");
      }
    );
  };

  refresh = apiType => {
    this.setState(
      { [apiType]: { ...this.state[apiType], isExecuting: true } },
      () => {
        this.props.context.api
          .get("/v1/events?offset=0&limit=100&orderBy=DESC")
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
    let { loadApi, refreshApi } = this.state;

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
                <ReportTable />
              )}
            </CardContent>
          </Card>
          {userCanView() && (
            <Fab
              color="secondary"
              className={classes.fab}
              onClick={this.handleAddClick}
            >
              <Add />
            </Fab>
          )}
        </ContentWrapper>
      </div>
    );
  }
}

EventReport.propTypes = {
  classes: PropTypes.object.isRequired
};

export default withStyles(styles)(withContext(EventReport));
