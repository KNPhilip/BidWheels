'use server'

import { Auction, PagedResult } from "@/types";

export async function getData(pageNumber: number = 1): Promise<PagedResult<Auction>> {
    const res: Response = await fetch(`http://localhost:6001/search?pageSize=4&pageNumber=${pageNumber}`);

    if (res.ok) throw new Error('Failed to fetch data.')

    return res.json();
}