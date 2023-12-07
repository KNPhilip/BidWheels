import React from 'react'

async function getData() {
    const res = await fetch('http://localhost:6001/search');

    if (!res.ok) throw new Error('Failed to fetch data.')

    return res.json();
}

const Listings = async () => {
    const data = await getData();

    return (
        <div>
            {JSON.stringify(data, null, 2)}
        </div>
    )
}

export default Listings;