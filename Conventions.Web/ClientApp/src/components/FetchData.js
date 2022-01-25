import React, { Component } from 'react';

export class FetchData extends Component {
  static displayName = FetchData.name;

  constructor(props) {
    super(props);
    this.state = { events: [], loading: true };
  }

  componentDidMount() {
    this.populateWeatherData();
  }

  static renderForecastsTable(events) {
    return (
      <table className='table table-striped' aria-labelledby="tabelLabel">
        <thead>
          <tr>
            <th>id</th>
            <th>name</th>
            <th>start date</th>
            <th>end date</th>
            <th>venue id</th>
          </tr>
        </thead>
        <tbody>
          {events.map(e =>
            <tr key={e.id}>
              <td>{e.id}</td>
              <td>{e.name}</td>
              <td>{new Date(e.startDate).toString()}</td>
              <td>{new Date(e.endDate).toString()}</td>
              <td>{e.venueId}</td>
            </tr>
          )}
        </tbody>
      </table>
    );
  }

  render() {
    let contents = this.state.loading
      ? <p><em>Loading...</em></p>
      : FetchData.renderForecastsTable(this.state.events);

    return (
      <div>
        <h1 id="label" >Events</h1>
        {contents}
      </div>
    );
  }

  async populateWeatherData() {
    const response = await fetch('api/events');
    const data = await response.json();
    this.setState({ events: data, loading: false });
  }
}
