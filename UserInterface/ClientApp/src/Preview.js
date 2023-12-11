import React, { useState, useEffect } from 'react';
import './Preview.css';
import Dashboard from './Dashboard';

const DeliveryList = () => {
    const [deliveries, setDeliveries] = useState([]);

    useEffect(() => {
        // Fetch data from the API endpoint
        fetch('http://localhost:5062/api/deliveries')
            .then(response => response.json())
            .then(data => {
                // Update the state with the fetched data
                setDeliveries(data);
            })
            .catch(error => {
                console.error('Error fetching data:', error);
            });
    }, []); // Empty dependency array ensures that the effect runs only once, similar to componentDidMount

    // Function to format the delivery time
    const formatDeliveryTime = (timeString) => {
        const options = {
            hour: 'numeric',
            minute: 'numeric',
            second: 'numeric',
            day: 'numeric',
            month: 'numeric',
            year: 'numeric',
        };

        const formattedTime = new Date(timeString).toLocaleString('sl-SI', options);
        return formattedTime;
    };

    return (
        <div className='Preview'>
            <Dashboard />
            <div className='preview-container'>
                <h2 className='header'>Deliveries</h2>
                <ul>
                    {deliveries.map(item => (
                        <li key={item.id} className='deliveryItem'>
                            <p>User ID: {item.userId}</p>
                            <p>Payment ID: {item.paymentId}</p>
                            <p>Address: {item.address}</p>
                            <p>Delivery Time: {formatDeliveryTime(item.deliveryTime)}</p>
                            <p>GeoX: {item.geoX}</p>
                            <p>GeoY: {item.geoY}</p>
                        </li>
                    ))}
                </ul>
            </div>
        </div>
    );
};

export default DeliveryList;
