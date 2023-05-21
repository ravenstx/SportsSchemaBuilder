import React, { useState, useEffect, useRef } from 'react';
import Highcharts from 'highcharts';
import HighchartsReact from 'highcharts-react-official';

(function (H) {
  H.Pointer.prototype.reset = function () {
    return undefined;
  };

  /**
   * Highlight a point by showing tooltip, setting hover state and draw crosshair
   */
  H.Point.prototype.highlight = function (event) {
    event = this.series.chart.pointer.normalize(event);
    this.onMouseOver(); // Show the hover marker
    //this.series.chart.tooltip.refresh(this); // Show the tooltip
    this.series.chart.xAxis[0].drawCrosshair(event, this); // Show the crosshair
  };

  H.syncExtremes = function (e) {
    var thisChart = this.chart;

    if (e.trigger !== 'syncExtremes') {
      // Prevent feedback loop
      Highcharts.each(Highcharts.charts, function (chart) {
        if (chart && chart !== thisChart) {
          if (chart.xAxis[0].setExtremes) {
            // It is null while updating
            chart.xAxis[0].setExtremes(e.min, e.max, undefined, false, {
              trigger: 'syncExtremes',
            });
          }
        }
      });
    }
  };
})(Highcharts);

function Charts(props) {
  const chartRef = useRef(null);
  const speedRef = useRef(null);
  const elevationRef = useRef(null);
  const heartRateRef = useRef(null);
  const [speedData, setSpeedData] = useState();
  const [elevationData, setElevationData] = useState();
  const [heartRateData, setHeartRateData] = useState();

  const getChartData = () => {
    const getSpeedData = props.data.map(function (val, i) {
      return props.data[i].records.map(function (val, j) {
        return [
          props.data[i].records[j]['distance'],
          props.data[i].records[j]['speed'],
        ];
      });
    });
    const getElevationData = props.data.map(function (val, i) {
      return props.data[i].records.map(function (val, j) {
        return [
          props.data[i].records[j]['distance'],
          props.data[i].records[j]['altitude'] * 1000,
        ];
      });
    });

    if (props.avg_heart_rate) {
      const getHeartRateData = props.data.map(function (val, i) {
        return props.data[i].records.map(function (val, j) {
          return [
            props.data[i].records[j]['distance'],
            props.data[i].records[j]['heart_rate'],
          ];
        });
      });

      setHeartRateData(getHeartRateData.flat(1));
    } else {
      setHeartRateData(null);
    }
    setSpeedData(getSpeedData.flat(1));
    setElevationData(getElevationData.flat(1));
  };

  const getOptions = (type, data, name, unit, valueDecimals, color) => ({
    chart: {
      type,
      height: 220,
    },
    title: {
      text: name,
      align: 'left',
      margin: 0,
      x: 30,
      style: {
        // fontWeight: 'bold',
      },
    },
    tooltip: {
      enabled: true,
      positioner: function () {
        return {
          // right aligned
          x: chartRef.current.offsetWidth - this.label.width,
          y: 0, // align to title
        };
      },
      borderWidth: 0,
      backgroundColor: 'none',
      pointFormat: '{point.y}',
      headerFormat: '',
      shadow: false,
      style: {
        fontSize: '18px',
      },
      valueDecimals: valueDecimals,
    },
    xAxis: {
      visible: true,
      ordinal: true,
      events: {
        setExtremes: function (e) {
          Highcharts.syncExtremes(e);
        },
      },
      crosshair: true,

      labels: {
        format: '{value} km',
      },
      accessibility: {
        description: 'Kilometers',
      },
    },
    yAxis: {
      title: {
        text: null,
      },
    },
    credits: {
      enabled: false,
    },
    legend: {
      enabled: false,
    },
    series: [
      {
        data: data,
        color: color,
        fillOpacity: 0.3,
        tooltip: {
          valueSuffix: ' ' + unit,
        },
      },
    ],
  });

  useEffect(() => {
    getChartData();
    if (speedRef != null) {
      ['mousemove', 'touchmove', 'touchstart'].forEach(function (eventType) {
        document
          .getElementById('chart-container')
          .addEventListener(eventType, function (e) {
            var chart, point, i, event;

            for (i = 0; i < Highcharts.charts.length; i = i + 1) {
              chart = Highcharts.charts[i];
              if (chart) {
                // Find coordinates within the chart
                event = chart.pointer.normalize(e);
                // Get the hovered point
                chart.series.forEach((series) => {
                  const point = series.searchPoint(event, true);
                  if (point) {
                    try {
                      point.highlight(e);
                    } catch (err) {
                      // pass;
                    }
                  }
                });
              }
            }
          });
      });
    }
  }, [props.data]);

  return (
    <>
      <figure className="highcharts-figure" ref={chartRef}>
        <div id="chart-container">
          <HighchartsReact
            ref={speedRef}
            highcharts={Highcharts}
            options={getOptions('line', speedData, 'speed', 'km/h', 1, '')}
          />
          <HighchartsReact
            ref={elevationRef}
            highcharts={Highcharts}
            options={getOptions(
              'area',
              elevationData,
              'elevation',
              'm',
              0,
              'black'
            )}
          />
          {heartRateData == null ? null : (
            <HighchartsReact
              ref={heartRateRef}
              highcharts={Highcharts}
              options={getOptions(
                'line',
                heartRateData,
                'heart rate',
                'bpm',
                0,
                '#F15A29'
              )}
            />
          )}
        </div>
      </figure>
    </>
  );
}

export default Charts;
