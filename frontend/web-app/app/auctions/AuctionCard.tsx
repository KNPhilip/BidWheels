import React from 'react'
import Image from 'next/image'

type Props = {
    auction: any
}

const AuctionCard = ({auction}: Props) => {
  return (
    <a href="#">
        <div className="w-full bg-gray-200 aspect-w-16 aspect-h-10 rounded-lg overflow-hidden">
            <div>
                <Image 
                    src={auction.imageUrl}
                    alt="image"
                    fill
                    priority
                    className="object-cover"
                    sizes='(max-width:768px) 100vw, (max-width: 1200px) 50vw, 25vw'
                />
            </div>
            <div className="flex justify-between items-center mt-4">
                <h3 className="text-gray-700">{auction.make} {auction.model}</h3>
                <p className="font-semibold text-sm">{auction.year}</p>
            </div>
        </div>
    </a>
  )
}

export default AuctionCard;