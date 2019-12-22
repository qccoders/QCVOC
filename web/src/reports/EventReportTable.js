import React from "react";
import PropTypes from "prop-types";
import moment from 'moment';

import { withStyles } from "@material-ui/core/styles";

import { 
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow
} from "@material-ui/core";

const styles = theme => ({
  table: {
    minWidth: 700
  }
});

const getDate = (date) => {
  date = moment(date);
  return date.local().format('MM/DD/YYYY');
};

const EventReportTable = (props) => {
  const { classes, data } = props;

  const sorted = data.sort((a, b) => new Date(a.startDate) - new Date(b.startDate));

  return (
    <Table className={classes.table}>
      <TableHead>
        <TableRow>
          <TableCell>Date</TableCell>
          <TableCell align="right">Event</TableCell>
          <TableCell align="right">Checkins</TableCell>
          <TableCell align="right">Plus 1</TableCell>
          <TableCell align="right">Total Scans</TableCell>
        </TableRow>
      </TableHead>
      <TableBody>
        {sorted.map((row, index) => (
          <TableRow key={index} hover>
            <TableCell component="th" scope="row">
              {getDate(row.startDate)}
            </TableCell>
            <TableCell align="right">{row.name}</TableCell>
            <TableCell align="right">{row.checkins}</TableCell>
            <TableCell align="right">{row.plusOnes}</TableCell>
            <TableCell align="right">{row.scans}</TableCell>
          </TableRow>
        ))}
      </TableBody>
    </Table>
  );
}

EventReportTable.propTypes = {
  classes: PropTypes.object.isRequired
};

export default withStyles(styles)(EventReportTable);
