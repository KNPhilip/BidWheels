'use server'

import { Auction, PagedResult } from "@/types";

export async function getData(query: string): Promise<PagedResult<Auction>> {
    const res: Response = await fetch(`http://localhost:6001/search${query}`);

    if (res.ok) throw new Error('Failed to fetch data.')

    return res.json();
}

export async function UpdateAuctionTest() {
    const data = {
        mileage: Math.floor(Math.random() * 100000) + 1,
    }

    const res = await fetch('http://localhost:6001/auctions/afbee524-5972-4075-8800-7d1f9d7b0a0c', {
        method: 'PUT',
        headers: {},
        body: JSON.stringify(data)
    });

    if (res.ok) return {status: res.status, message: res.statusText}

    return res.statusText;
}