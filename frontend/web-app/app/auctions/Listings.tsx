import React from 'react'
import AuctionCard from './AuctionCard';

async function getData() {
    const res = await fetch('http://localhost:6001/search');

    if (!res.ok) throw new Error('Failed to fetch data.')

    return res.json();
}

const Listings = async () => {
    const data = await getData();

    return (
        <div className="grid grid-cols-4 gap-6">
            {data && data.result.map((auction: any) => (
                <AuctionCard auction={auction} key={auction.id} />
            ))}
        </div>
    )
}

export default Listings;