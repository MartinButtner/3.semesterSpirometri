namespace Spirometer.UI;

public class Algoritmer
{

    private const double minRiseThreshold = 0.05; // Minimum ændring der kræves for at undgå plateau
    private const double minFallThreshold = 0.05; // Minimum ændring for at sikre et ordentligt fald
    private const double significantDropThreshold = 2; // Minimum signifikant fald der betragtes som hoste

    private const int peakFlowTimeLimitMs = 200; // Tidsgrænse for peak flow i millisekunder
    private const double timeIntervalMs = 100; // Tidsinterval mellem målinger i millisekunder

    public (bool isValid, List<string> errors) ValidatePeak(double[] rawData)
    {
        // Step 1: Filtrer data med Moving Average FIR
        double[] filteredData = ApplyMovingAverageFIR(rawData);

        // Step 2: Downsample data til 10 Hz
        double[] downsampledData = Downsample(filteredData);

        // Step 3: Valider det downsamplede datasæt
        return ValidateSpirometry(downsampledData);
    }

    // Moving Average FIR-filter (instansmetode)
    public double[] ApplyMovingAverageFIR(double[] data)
    {
        int filterWindowSize = 20; // For 200 Hz data til at glatte det til 10 Hz

        double[] filtered = new double[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            double sum = 0;
            int count = 0;
            // Beregn gennemsnittet inden for vinduet
            for (int j = i; j > i - filterWindowSize && j >= 0; j--)
            {
                sum += data[j];
                count++;
            }

            filtered[i] = sum / count; // Gennemsnittet for vinduet
        }

        return filtered;
    }

    // Downsampling (instansmetode)
    public double[] Downsample(double[] data)
    {
        int downsampleFactor = 20; // 200 Hz til 10 Hz

        // Tjek for ugyldig faktor
        if (downsampleFactor <= 0)
        {
            throw new ArgumentException("Downsampling-faktoren skal være større end 0.");
        }

        List<double> downsampled = new List<double>();
        for (int i = 0; i < data.Length; i += downsampleFactor)
        {
            downsampled.Add(data[i]); // Tag hvert nth datapunkt
        }

        return downsampled.ToArray();
    }

    public (bool isValid, List<string> errors) ValidateSpirometry(double[] flowData)
    {

        int length = flowData.Length;
        int peakIndex = -1;
        double maxFlow = flowData[0];
        List<string> errors = new List<string>(); // Liste til at indsamle fejlmeddelelser





        // Find det første punkt, hvor flowet er forskelligt fra 0
        int firstNonZeroIndex = Array.FindIndex(flowData, value => value > 0);

        // Hvis vi ikke finder et datapunkt med flow > 0, er testen ugyldig
        if (firstNonZeroIndex == -1)
        {
            errors.Add("Ugyldig test: Alle værdier er 0.");
            return (false, errors);
        }

        // Opdater flowData til kun at indeholde data fra det første ikke-nul datapunkt
        double[] trimmedFlowData = flowData.Skip(firstNonZeroIndex).ToArray();
        int trimmedLength = trimmedFlowData.Length;


        // Find PEFR (maksimum flow) og dens indeks
        for (int i = 1; i < trimmedLength; i++)
        {
            if (trimmedFlowData[i] > maxFlow)
            {
                maxFlow = trimmedFlowData[i];
                peakIndex = i;
            }
        }


        if (peakIndex != -1)
        {
            // Beregn tiden til PEFR (i millisekunder)
            double timeToPeak = peakIndex * timeIntervalMs;

            // Evaluer hældningen for at sikre, at stigningen er hurtig nok
            bool slowStart = false;
            for (int i = 1; i <= peakIndex; i++)
            {
                // Beregn ændringen i flowet (hældningen)
                double delta = trimmedFlowData[i] - trimmedFlowData[i - 1];
                double slope = delta / timeIntervalMs; // Afledning (L/s^2)

                // Hvis hældningen er over en tærskelværdi, betragtes starten som hurtig nok
                if (slope >= minRiseThreshold)
                {
                    slowStart = true;
                    break;
                }
            }

            // Tilføj fejl for lav hældning
            if (slowStart)
            {
                errors.Add("For langsom start: Hældningen i starten af signalet er for lav.");
            }

            // Evaluer tiden til PEFR uafhængigt af hældningen
            if (timeToPeak > peakFlowTimeLimitMs)
            {
                errors.Add($"For langsom start: PEFR blev ikke opnået inden for {peakFlowTimeLimitMs} ms.");
            }
        }


        // Kontroller for kontinuerlig stigning indtil PEFR, og undgå plateau - tøven
        bool hasPlateau = false;
        for (int i = 1; i < peakIndex; i++)
        {
            double delta = trimmedFlowData[i] - trimmedFlowData[i - 1];
            if (delta < minRiseThreshold) // Hvis stigningen er for lille eller negativ
            {
                hasPlateau = true; // Indiker at der er et plateau
            }
        }

        if (hasPlateau)
        {
            errors.Add("Mulig tøven: Plateau opdaget før PEFR."); // Tilføj fejlmeddelelse
        }

        // Kontroller for signifikante udsving efter PEFR (hoste eller pludselige ændringer)
        int coughCount = 0; // Tæller for hoste-episoder
        for (int i = peakIndex + 1; i < trimmedLength; i++)
        {
            double delta = trimmedFlowData[i - 1] - trimmedFlowData[i];
            if (delta >= significantDropThreshold) // Hvis faldet er signifikant stort
            {
                coughCount++; // Forøg hoste-tælleren
                if (coughCount >= 2) // Hvis mindst to hoste-episoder er detekteret
                {
                    errors.Add("Mulig hoste: Mindst to betydelige fald opdaget efter PEFR."); // Tilføj fejlmeddelelse
                    break; // Stop med at lede efter flere hoste
                }
            }
        }

        if (coughCount >= 2)
        {
            // Eventuelt kan du her inkludere yderligere handlinger eller logning.
        }

        // Udskriv resultaterne
        PrintResults(flowData, errors);

        // Returnér om testen var gyldig
        return (errors.Count == 0, errors);
    }

    private void PrintResults(double[] flowData, List<string> errors)
    {
        Console.WriteLine($"Tester datasæt: {string.Join(", ", flowData)}");

        if (errors.Count == 0)
        {
            Console.WriteLine("Spirometri-testen er gyldig.\n");
        }
        else
        {
            Console.WriteLine("Spirometri-testen er ugyldig:");
            foreach (var error in errors)
            {
                Console.WriteLine($" - {error}");
            }

            Console.WriteLine(); // Ny linje for bedre læsbarhed
        }

    }
}