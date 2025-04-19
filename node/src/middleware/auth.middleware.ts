import { Request, Response, NextFunction } from "express"
import jwt from "jsonwebtoken"

export const authenticateToken = (req: Request, res: Response, next: NextFunction) => {
    
    const token = req.header("Authorization")?.split(" ")[1]

    if (!token) {
        res.status(401).json({ message: "Not authorized!" })
        return
    }

    jwt.verify(token, process.env.JWT_SECRET as string, { ignoreExpiration: true }, (err, user) => {
        if (err) {
            res.status(401).json({ message: "Invalid token" })
            return
        } else {
            (req as any).user = user
            next()
        }
    })
}
