import React, { useState, useEffect } from 'react';
import './Rabbit.css';
import Dashboard from './Dashboard';

const Rabbit = () => {
    const [logs, setLogs] = useState([]);
    const [logsBetweenDates, setLogsBetweenDates] = useState([]);
    const [startDate, setStartDate] = useState('');
    const [endDate, setEndDate] = useState('');

    useEffect(() => {
        // Fetch data from the API endpoint (optional, if you want to display existing logs)
        fetch('https://localhost:7136/Logging/logs')
            .then(response => response.json())
            .then(data => {
                setLogs(data);
            })
            .catch(error => {
                console.error('Error fetching logs:', error);
            });
    }, []); // Empty dependency array ensures the effect runs once on mount

    const sendLogsToMongo = async () => {
        try {
            const response = await fetch('https://localhost:7136/Logging/postLogs', {
                method: 'POST',
                headers: {
                    'accept': '*/*',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({}),  // Replace with actual data if needed
            });

            if (!response.ok) {
                throw new Error('Failed to send logs to MongoDB');
            }

            // Handle success
            console.log('Logs sent successfully');

            // Optional: Fetch updated logs after sending
            const updatedLogs = await fetch('https://localhost:7136/Logging/logs')
                .then(response => response.json())
                .catch(error => {
                    console.error('Error fetching logs:', error);
                });

            setLogs(updatedLogs || []); // Update logs state
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const getLogsBetweenDates = async () => {
        try {
            const response = await fetch(`https://localhost:7136/Logging/logs/${startDate}/${endDate}`, {
                method: 'GET',
                headers: {
                    'accept': '*/*'
                },
            });

            if (!response.ok) {
                throw new Error('Failed to fetch logs between dates');
            }

            const data = await response.json();
            setLogsBetweenDates(data); // Update logsBetweenDates state

            // Handle the fetched data as needed
            console.log('Logs between dates:', data);
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    const deleteAllLogs = async () => {
        try {
            const response = await fetch('https://localhost:7136/Logging/clearLogs', {
                method: 'DELETE',
                headers: {
                    'accept': '*/*'
                },
            });

            if (!response.ok) {
                throw new Error('Failed to delete all logs');
            }

            // Handle success
            console.log('All logs deleted successfully');

            // Clear logs and logsBetweenDates state after deletion
            setLogs([]);
            setLogsBetweenDates([]);
        } catch (error) {
            console.error('Error:', error.message);
        }
    };

    return (
        <div className="Rabbit">
            <Dashboard />
            <div className="preview-container">
                <h2>Logs</h2>
                {logs.length === 0 ? (
                    <p>No logs present.</p>
                ) : (
                    <ul>
                        {logs.map(log => (
                            <li key={log.id} className="logItem">
                                {/* Display log details as needed */}
                                <p>Timestamp: {log.timestamp}</p>
                                <p>Log Type: {log.logType}</p>
                                {/* Add more log details as needed */}
                            </li>
                        ))}
                    </ul>
                )}
                <button onClick={sendLogsToMongo}>Send Logs to MongoDB</button>
                <div>
                    <label>Start Date:</label>
                    <input type="date" value={startDate} onChange={(e) => setStartDate(e.target.value)} />
                    <label>End Date:</label>
                    <input type="date" value={endDate} onChange={(e) => setEndDate(e.target.value)} />
                    <button onClick={getLogsBetweenDates}>Get Logs Between Dates</button>
                </div>
                <button onClick={deleteAllLogs}>Delete All Logs</button>

                {/* Display logs between dates if available */}
                {logsBetweenDates.length > 0 && (
                    <div>
                        <h2>Logs Between Dates</h2>
                        <ul>
                            {logsBetweenDates.map(log => (
                                <li key={log.id} className="logItem">
                                    {/* Display log details as needed */}
                                    <p>Timestamp: {log.timestamp}</p>
                                    <p>Log Type: {log.logType}</p>
                                    {/* Add more log details as needed */}
                                </li>
                            ))}
                        </ul>
                    </div>
                )}
            </div>
        </div>
    );
};

export default Rabbit;
