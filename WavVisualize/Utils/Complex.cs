using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

public struct Complex : ICloneable, ISerializable
{
    public double Re;

    public double Im;

    public static readonly Complex Zero = new Complex(0, 0);

    public static readonly Complex One = new Complex(1, 0);

    public static readonly Complex I = new Complex(0, 1);

    public double Magnitude
    {
        get { return Math.Sqrt(Re * Re + Im * Im); }
    }

    public double Phase
    {
        get { return Math.Atan2(Im, Re); }
    }

    public double SquaredMagnitude
    {
        get { return (Re * Re + Im * Im); }
    }

    public Complex(double re, double im)
    {
        this.Re = re;
        this.Im = im;
    }

    public Complex(Complex c)
    {
        this.Re = c.Re;
        this.Im = c.Im;
    }

    public static Complex Add(Complex a, Complex b)
    {
        return new Complex(a.Re + b.Re, a.Im + b.Im);
    }

    public static Complex Add(Complex a, double s)
    {
        return new Complex(a.Re + s, a.Im);
    }

    public static void Add(Complex a, Complex b, ref Complex result)
    {
        result.Re = a.Re + b.Re;
        result.Im = a.Im + b.Im;
    }

    public static void Add(Complex a, double s, ref Complex result)
    {
        result.Re = a.Re + s;
        result.Im = a.Im;
    }

    public static Complex Subtract(Complex a, Complex b)
    {
        return new Complex(a.Re - b.Re, a.Im - b.Im);
    }

    public static Complex Subtract(Complex a, double s)
    {
        return new Complex(a.Re - s, a.Im);
    }

    public static Complex Subtract(double s, Complex a)
    {
        return new Complex(s - a.Re, a.Im);
    }

    public static void Subtract(Complex a, Complex b, ref Complex result)
    {
        result.Re = a.Re - b.Re;
        result.Im = a.Im - b.Im;
    }

    public static void Subtract(Complex a, double s, ref Complex result)
    {
        result.Re = a.Re - s;
        result.Im = a.Im;
    }

    public static void Subtract(double s, Complex a, ref Complex result)
    {
        result.Re = s - a.Re;
        result.Im = a.Im;
    }

    public static Complex Multiply(Complex a, Complex b)
    {
        // (x + yi)(u + vi) = (xu  yv) + (xv + yu)i. 
        double aRe = a.Re, aIm = a.Im;
        double bRe = b.Re, bIm = b.Im;

        return new Complex(aRe * bRe - aIm * bIm, aRe * bIm + aIm * bRe);
    }

    public static Complex Multiply(Complex a, double s)
    {
        return new Complex(a.Re * s, a.Im * s);
    }

    public static void Multiply(Complex a, Complex b, ref Complex result)
    {
        // (x + yi)(u + vi) = (xu  yv) + (xv + yu)i. 
        double aRe = a.Re, aIm = a.Im;
        double bRe = b.Re, bIm = b.Im;

        result.Re = aRe * bRe - aIm * bIm;
        result.Im = aRe * bIm + aIm * bRe;
    }

    public static void Multiply(Complex a, double s, ref Complex result)
    {
        result.Re = a.Re * s;
        result.Im = a.Im * s;
    }

    public static Complex Divide(Complex a, Complex b)
    {
        double aRe = a.Re, aIm = a.Im;
        double bRe = b.Re, bIm = b.Im;
        double modulusSquared = bRe * bRe + bIm * bIm;

        if (modulusSquared == 0)
        {
            throw new DivideByZeroException("Can not divide by zero.");
        }

        double invModulusSquared = 1 / modulusSquared;

        return new Complex(
            (aRe * bRe + aIm * bIm) * invModulusSquared,
            (aIm * bRe - aRe * bIm) * invModulusSquared);
    }

    public static Complex Divide(Complex a, double s)
    {
        if (s == 0)
        {
            throw new DivideByZeroException("Can not divide by zero.");
        }

        return new Complex(a.Re / s, a.Im / s);
    }

    public static Complex Divide(double s, Complex a)
    {
        if ((a.Re == 0) || (a.Im == 0))
        {
            throw new DivideByZeroException("Can not divide by zero.");
        }

        return new Complex(s / a.Re, s / a.Im);
    }

    public static void Divide(Complex a, Complex b, ref Complex result)
    {
        double aRe = a.Re, aIm = a.Im;
        double bRe = b.Re, bIm = b.Im;
        double modulusSquared = bRe * bRe + bIm * bIm;

        if (modulusSquared == 0)
        {
            throw new DivideByZeroException("Can not divide by zero.");
        }

        double invModulusSquared = 1 / modulusSquared;

        result.Re = (aRe * bRe + aIm * bIm) * invModulusSquared;
        result.Im = (aIm * bRe - aRe * bIm) * invModulusSquared;
    }

    public static void Divide(Complex a, double s, ref Complex result)
    {
        if (s == 0)
        {
            throw new DivideByZeroException("Can not divide by zero.");
        }

        result.Re = a.Re / s;
        result.Im = a.Im / s;
    }

    public static void Divide(double s, Complex a, ref Complex result)
    {
        if ((a.Re == 0) || (a.Im == 0))
        {
            throw new DivideByZeroException("Can not divide by zero.");
        }

        result.Re = s / a.Re;
        result.Im = s / a.Im;
    }

    public static Complex Negate(Complex a)
    {
        return new Complex(-a.Re, -a.Im);
    }

    public static bool ApproxEqual(Complex a, Complex b)
    {
        return ApproxEqual(a, b, 8.8817841970012523233891E-16);
    }

    public static bool ApproxEqual(Complex a, Complex b, double tolerance)
    {
        return
        (
            (Math.Abs(a.Re - b.Re) <= tolerance) &&
            (Math.Abs(a.Im - b.Im) <= tolerance)
        );
    }

    #region Public Static Parse Methods

    public static Complex Parse(string s)
    {
        Regex r = new Regex(@"\((?<real>.*),(?<imaginary>.*)\)", RegexOptions.None);
        Match m = r.Match(s);

        if (m.Success)
        {
            return new Complex(
                double.Parse(m.Result("${real}")),
                double.Parse(m.Result("${imaginary}"))
            );
        }
        else
        {
            throw new FormatException("String representation of the complex number is not correctly formatted.");
        }
    }

    public static bool TryParse(string s, out Complex result)
    {
        try
        {
            Complex newComplex = Complex.Parse(s);
            result = newComplex;
            return true;
        }
        catch (FormatException)
        {
            result = new Complex();
            return false;
        }
    }

    #endregion

    #region Public Static Complex Special Functions

    public static Complex Sqrt(Complex a)
    {
        Complex result = Complex.Zero;

        if ((a.Re == 0.0) && (a.Im == 0.0))
        {
            return result;
        }
        else if (a.Im == 0.0)
        {
            result.Re = (a.Re > 0) ? Math.Sqrt(a.Re) : Math.Sqrt(-a.Re);
            result.Im = 0.0;
        }
        else
        {
            double modulus = a.Magnitude;

            result.Re = Math.Sqrt(0.5 * (modulus + a.Re));
            result.Im = Math.Sqrt(0.5 * (modulus - a.Re));
            if (a.Im < 0.0)
                result.Im = -result.Im;
        }

        return result;
    }

    public static Complex Log(Complex a)
    {
        Complex result = Complex.Zero;

        if ((a.Re > 0.0) && (a.Im == 0.0))
        {
            result.Re = Math.Log(a.Re);
            result.Im = 0.0;
        }
        else if (a.Re == 0.0)
        {
            if (a.Im > 0.0)
            {
                result.Re = Math.Log(a.Im);
                result.Im = Math.PI / 2.0;
            }
            else
            {
                result.Re = Math.Log(-(a.Im));
                result.Im = -Math.PI / 2.0;
            }
        }
        else
        {
            result.Re = Math.Log(a.Magnitude);
            result.Im = Math.Atan2(a.Im, a.Re);
        }

        return result;
    }

    public static Complex Exp(Complex a)
    {
        Complex result = Complex.Zero;
        double r = Math.Exp(a.Re);
        result.Re = r * Math.Cos(a.Im);
        result.Im = r * Math.Sin(a.Im);

        return result;
    }

    #endregion

    #region Public Static Complex Trigonometry

    public static Complex Sin(Complex a)
    {
        Complex result = Complex.Zero;

        if (a.Im == 0.0)
        {
            result.Re = Math.Sin(a.Re);
            result.Im = 0.0;
        }
        else
        {
            result.Re = Math.Sin(a.Re) * Math.Cosh(a.Im);
            result.Im = Math.Cos(a.Re) * Math.Sinh(a.Im);
        }

        return result;
    }

    public static Complex Cos(Complex a)
    {
        Complex result = Complex.Zero;

        if (a.Im == 0.0)
        {
            result.Re = Math.Cos(a.Re);
            result.Im = 0.0;
        }
        else
        {
            result.Re = Math.Cos(a.Re) * Math.Cosh(a.Im);
            result.Im = -Math.Sin(a.Re) * Math.Sinh(a.Im);
        }

        return result;
    }

    public static Complex Tan(Complex a)
    {
        Complex result = Complex.Zero;

        if (a.Im == 0.0)
        {
            result.Re = Math.Tan(a.Re);
            result.Im = 0.0;
        }
        else
        {
            double real2 = 2 * a.Re;
            double imag2 = 2 * a.Im;
            double denom = Math.Cos(real2) + Math.Cosh(real2);

            result.Re = Math.Sin(real2) / denom;
            result.Im = Math.Sinh(imag2) / denom;
        }

        return result;
    }

    #endregion

    #region Overrides

    public override int GetHashCode()
    {
        return Re.GetHashCode() ^ Im.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return (obj is Complex) ? (this == (Complex) obj) : false;
    }

    public override string ToString()
    {
        return string.Format("({0}, {1})", Re, Im);
    }

    #endregion

    #region Comparison Operators

    public static bool operator ==(Complex u, Complex v)
    {
        return ((u.Re == v.Re) && (u.Im == v.Im));
    }

    public static bool operator !=(Complex u, Complex v)
    {
        return !(u == v);
    }

    #endregion

    #region Unary Operators

    public static Complex operator -(Complex a)
    {
        return Complex.Negate(a);
    }

    #endregion

    #region Binary Operators

    public static Complex operator +(Complex a, Complex b)
    {
        return Complex.Add(a, b);
    }

    public static Complex operator +(Complex a, double s)
    {
        return Complex.Add(a, s);
    }

    public static Complex operator +(double s, Complex a)
    {
        return Complex.Add(a, s);
    }

    public static Complex operator -(Complex a, Complex b)
    {
        return Complex.Subtract(a, b);
    }

    public static Complex operator -(Complex a, double s)
    {
        return Complex.Subtract(a, s);
    }

    public static Complex operator -(double s, Complex a)
    {
        return Complex.Subtract(s, a);
    }

    public static Complex operator *(Complex a, Complex b)
    {
        return Complex.Multiply(a, b);
    }

    public static Complex operator *(double s, Complex a)
    {
        return Complex.Multiply(a, s);
    }

    public static Complex operator *(Complex a, double s)
    {
        return Complex.Multiply(a, s);
    }

    public static Complex operator /(Complex a, Complex b)
    {
        return Complex.Divide(a, b);
    }

    public static Complex operator /(Complex a, double s)
    {
        return Complex.Divide(a, s);
    }

    public static Complex operator /(double s, Complex a)
    {
        return Complex.Divide(s, a);
    }

    #endregion

    #region Conversion Operators

    public static explicit operator Complex(float value)
    {
        return new Complex((double) value, 0);
    }

    public static explicit operator Complex(double value)
    {
        return new Complex(value, 0);
    }

    #endregion

    #region ICloneable Members

    object ICloneable.Clone()
    {
        return new Complex(this);
    }

    public Complex Clone()
    {
        return new Complex(this);
    }

    #endregion

    #region ISerializable Members

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("Real", this.Re);
        info.AddValue("Imaginary", this.Im);
    }

    #endregion
}