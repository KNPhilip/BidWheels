import type { Metadata } from 'next'
import './globals.css'

export const metadata: Metadata = {
  title: 'BidWheels',
  description: 'BidWheels is a car bidding application, build with Microservices architecture using .NET, NextJS, Duende Identity Server, EF Core, Docker, Kubernetes, RabbitMQ, gRPC, and more....',
}

export default function RootLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <html lang="en">
      <body>{children}</body>
    </html>
  )
}
