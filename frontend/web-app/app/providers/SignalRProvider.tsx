'use client'

import { useAuctionStore } from '@/hooks/useAuctionStore';
import { useBidStore } from '@/hooks/useBidStore';
import { Auction, Bid } from '@/types';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from 'next-auth';
import { ReactNode, useEffect, useState } from 'react'
import toast from 'react-hot-toast';
import AuctionCreatedToast from '../components/AuctionCreatedToast';

type Props = {
    children: ReactNode
    user: User | null
}

const SignalRProvider = ({children, user}: Props) => {
    const [connection, setConnection] = useState<HubConnection | null>(null);
    const setCurrentPrice = useAuctionStore(state => state.setCurrentPrice);
    const addBid = useBidStore(state => state.addBid);

    useEffect(() => {
        const newConnection = new HubConnectionBuilder()
            .withUrl('http://localhost:6001/notifications')
            .withAutomaticReconnect()
            .build();

        setConnection(newConnection);
    }, []);

    useEffect(() => {
        if (connection) {
            connection.start()
                .then(() => {
                    console.log('Connected to notification hub')

                    connection.on('BidPlaced', (bid: Bid) => {
                        if (bid.bidStatus.includes('Accepted')) {
                            setCurrentPrice(bid.auctionId, bid.amount);
                        }
                        addBid(bid);
                    });

                    connection.on('AuctionCreated', (auction: Auction) => {
                        if (user?.username !== auction.seller) {
                            return toast(<AuctionCreatedToast auction={auction} />, {duration: 5000});
                        }
                    });
                }).catch(err => console.error('SignalR connection failed', err));
        }

        return () => {
            connection?.stop()
        }
    }, [connection, setCurrentPrice]);

    return (
        children
    )
}

export default SignalRProvider;