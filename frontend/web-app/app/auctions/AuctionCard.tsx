import React from 'react'

type Props = {
    auction: any
}

const AuctionCard = ({auction}: Props) => {
  return (
    <div>{auction.auction.make}</div>
  )
}

export default AuctionCard;